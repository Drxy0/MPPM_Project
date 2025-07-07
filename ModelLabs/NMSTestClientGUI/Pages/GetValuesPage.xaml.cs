using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NMSTestClientGUI.Pages
{
    /// <summary>
    /// Interaction logic for GetValuesPage.xaml
    /// </summary>
    public partial class GetValuesPage : Page
    {
        public static TestGdaGUI testGda;
        public static ModelResourcesDesc resourcesDesc;
        public static Dictionary<ModelCode, List<ModelCode>> propertyIDsByModelCode;
        public static List<long> gids;
        public static ModelCode code;

        public GetValuesPage()
        {
            InitializeComponent();

            testGda = new TestGdaGUI();
            resourcesDesc = new ModelResourcesDesc();
            propertyIDsByModelCode = Enum.GetValues(typeof(ModelCode))
                .Cast<ModelCode>()
                .ToDictionary(mc => mc, mc => resourcesDesc.GetAllPropertyIds(mc));

            gids = testGda.TestGetExtentValuesAllTypes();
            GIDCombobox.ItemsSource = gids.Select(gid => $"0x{gid:x16}").ToList();
        }

        private void GIDCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GIDCombobox.SelectedIndex >= 0)
            {
                code = resourcesDesc.GetModelCodeFromId(GetGID());
                CreateCheckBoxes();
            }
        }
        private long GetGID() => gids[GIDCombobox.SelectedIndex];

        private void CreateCheckBoxes()
        {
            CheckBoxGrid.Children.Clear();

            var props = propertyIDsByModelCode[code];
            GetValuesButton.Visibility = Visibility.Visible;
            SelectAllCheckBox.Visibility = Visibility.Visible;
            SelectAllCheckBox.IsChecked = false;

            foreach (var prop in props)
            {
                var cb = new CheckBox
                {
                    Content = prop.ToString(),
                    Visibility = Visibility.Visible
                };

                CheckBoxGrid.Children.Add(cb);
            }
        }

        private void GetValuesButton_Click(object sender, RoutedEventArgs e)
        {
            List<ModelCode> properties = GetPropertiesForCheckedBoxes();

            ResourceDescription rd = testGda.GetValues(GetGID(), properties);
            WriteResultsToTextBox(rd);
        }

        private List<ModelCode> GetPropertiesForCheckedBoxes()
        {
            if (SelectAllCheckBox.IsChecked == true)
                return propertyIDsByModelCode[code];

            var selectedProps = new List<ModelCode>();

            foreach (var child in CheckBoxGrid.Children)
            {
                if (child is CheckBox cb && cb.IsChecked == true)
                {
                    if (Enum.TryParse(cb.Content.ToString(), out ModelCode prop))
                    {
                        selectedProps.Add(prop);
                    }
                }
            }

            return selectedProps;
        }

        private void WriteResultsToTextBox(ResourceDescription rd)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ResourceDescription:");
            sb.AppendLine($"Gid = 0x{rd.Id:x16}");
            sb.AppendLine($"Type = {(DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)}");
            sb.AppendLine("Properties:");

            foreach (Property p in rd.Properties)
            {
                sb.Append($"\t{p.Id} = ");

                switch (p.Type)
                {
                    case PropertyType.Float:
                        sb.Append(p.AsFloat());
                        break;
                    case PropertyType.Bool:
                        sb.Append(p.PropertyValue.FloatValue == 1 ? "True" : "False");
                        break;
                    case PropertyType.Byte:
                    case PropertyType.Int32:
                    case PropertyType.Int64:
                    case PropertyType.TimeSpan:
                    case PropertyType.DateTime:
                        sb.Append(p.Id == ModelCode.IDOBJ_GID ? $"0x{p.AsLong():x16}" : p.AsLong().ToString());
                        break;
                    case PropertyType.Reference:
                        sb.Append($"0x{p.AsReference():x16}");
                        break;
                    case PropertyType.String:
                        sb.Append(p.AsString() ?? string.Empty);
                        break;
                    case PropertyType.Int64Vector:
                    case PropertyType.ReferenceVector:
                        var longs = p.AsLongs();
                        sb.Append(longs.Any()
                            ? string.Join(", ", longs.Select(val => $"0x{val:x16}"))
                            : "empty long/reference vector");
                        break;
                    case PropertyType.Enum:
                        try
                        {
                            sb.Append(new EnumDescs().GetStringFromEnum(p.Id, p.AsEnum()));
                        }
                        catch
                        {
                            sb.Append(p.AsEnum());
                        }
                        break;
                    default:
                        throw new Exception("Invalid property type.");
                }

                sb.AppendLine();
            }

            ResultsTextBox.Text = sb.ToString();
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = SelectAllCheckBox.IsChecked == true;

            foreach (var child in CheckBoxGrid.Children)
            {
                if (child is CheckBox cb)
                    cb.IsChecked = isChecked;
            }
        }
    }
}
