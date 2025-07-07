using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NMSTestClientGUI.Pages
{
    /// <summary>
    /// Interaction logic for GetRelatedValuesPage.xaml
    /// </summary>
    public partial class GetRelatedValuesPage : Page
    {
        public static TestGdaGUI testGda;
        public static ModelResourcesDesc resourcesDesc;
        public static Dictionary<ModelCode, List<ModelCode>> propertyIDsByModelCode;
        public static List<long> allGids;
        public static List<ModelCode> associations;
        public static List<DMSType> dmsTypes;
        public static long gid;
        public static ModelCode code;
        public static DMSType dms;
        public static Association association;
        public static ModelCode assocCode;
        public static ModelCode dmsCode;
        public GetRelatedValuesPage()
        {
            InitializeComponent();

            testGda = new TestGdaGUI();
            resourcesDesc = new ModelResourcesDesc();
            propertyIDsByModelCode = Enum.GetValues(typeof(ModelCode))
                .Cast<ModelCode>()
                .ToDictionary(mc => mc, mc => resourcesDesc.GetAllPropertyIds(mc));

            allGids = testGda.TestGetExtentValuesAllTypes();
            GIDComboBox.ItemsSource = allGids.Select(g => $"0x{g:x16}").ToList();

            dmsTypes = Enum.GetValues(typeof(DMSType))
                .Cast<DMSType>()
                .Where(t => t != DMSType.MASK_TYPE)
                .ToList();
        }

        private void GIDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GIDComboBox.SelectedIndex < 0) return;
            gid = allGids[GIDComboBox.SelectedIndex];

            code = resourcesDesc.GetModelCodeFromId(gid);
            associations = resourcesDesc.GetAllPropertyIdsForEntityId(gid)
                .Where(mc => ((long)mc & 0xF) == 0x9) // only references
                .ToList();

            AssociationComboBox.ItemsSource = associations.Select(a => a.ToString()).ToList();
        }

        private void AssociationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociationComboBox.SelectedIndex < 0) return;

            assocCode = associations[AssociationComboBox.SelectedIndex];
            DMSTypeComboBox.ItemsSource = dmsTypes.Select(d => d.ToString()).ToList();
        }

        private void DMSTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DMSTypeComboBox.SelectedIndex < 0) return;

            dms = dmsTypes[DMSTypeComboBox.SelectedIndex];
            dmsCode = resourcesDesc.GetModelCodeFromType(dms);
            CreateCheckBoxes();
        }

        private void CreateCheckBoxes()
        {
            CheckBoxGrid.Children.Clear();

            var props = propertyIDsByModelCode[dmsCode];
            GetRelatedValuesButton.Visibility = Visibility.Visible;
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

        private List<ModelCode> GetPropertiesForCheckedBoxes()
        {
            if (SelectAllCheckBox.IsChecked == true)
                return propertyIDsByModelCode[dmsCode];

            var selectedProps = new List<ModelCode>();

            foreach (var child in CheckBoxGrid.Children)
            {
                if (child is CheckBox cb && cb.IsChecked == true && Enum.TryParse(cb.Content.ToString(), out ModelCode prop))
                {
                    selectedProps.Add(prop);
                }
            }

            return selectedProps;
        }
        private void GetRelatedValuesButton_Click(object sender, RoutedEventArgs e)
        {
            var properties = GetPropertiesForCheckedBoxes();

            association = new Association
            {
                Inverse = false,
                PropertyId = assocCode,
                Type = resourcesDesc.GetModelCodeFromType(dms)
            };

            var sb = new StringBuilder();
            sb.AppendLine($"ResourceDescriptions for {assocCode}:");

            var relatedGids = testGda.GetRelatedValues(gid, association, properties);

            foreach (var g in relatedGids)
            {
                var rd = testGda.GetValues(g, properties);
                sb.AppendLine();
                sb.AppendLine($"Gid = 0x{rd.Id:x16}");
                sb.AppendLine($"Type = {(DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)}");
                sb.AppendLine("Properties:");

                foreach (var p in rd.Properties)
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
