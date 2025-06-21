using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    public class FTN_93_ProfileImporter
    {
        private static FTN_93_ProfileImporter ptImporter = null;
        private static object singletoneLock = new object();

        private ConcreteModel concreteModel;
        private Delta delta;
        private ImportHelper importHelper;
        private TransformAndLoadReport report;

        #region Properties
        public static FTN_93_ProfileImporter Instance
        {
            get
            {
                if (ptImporter == null)
                {
                    lock (singletoneLock)
                    {
                        if (ptImporter == null)
                        {
                            ptImporter = new FTN_93_ProfileImporter();
                            ptImporter.Reset();
                        }
                    }
                }
                return ptImporter;
            }
        }

        public Delta NMSDelta
        {
            get
            {
                return delta;
            }
        }
        #endregion Properties
        public void Reset()
        {
            concreteModel = null;
            delta = new Delta();
            importHelper = new ImportHelper();
            report = null;
        }


        public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
        {
            LogManager.Log("Importing PowerTransformer Elements...", LogLevel.Info);
            report = new TransformAndLoadReport();
            concreteModel = cimConcreteModel;
            delta.ClearDeltaOperations();

            if ((concreteModel != null) && (concreteModel.ModelMap != null))
            {
                try
                {
                    // convert into DMS elements
                    ConvertModelAndPopulateDelta();
                }
                catch (Exception ex)
                {
                    string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
                    LogManager.Log(message);
                    report.Report.AppendLine(ex.Message);
                    report.Success = false;
                }
            }
            LogManager.Log("Importing PowerTransformer Elements - END.", LogLevel.Info);
            return report;
        }

        /// <summary>
        /// Method performs conversion of network elements from CIM based concrete model into DMS model.
        /// </summary>
        private void ConvertModelAndPopulateDelta()
        {
            LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

            //// import all concrete model types (DMSType enum)
            ImportSwitch();

            LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
        }


        #region Import

        private void ImportSwitch()
        {
            SortedDictionary<string, object> cimSwitches = concreteModel.GetAllObjectsOfType("FTN.Switch");
            if (cimSwitches is null)
            {
                return;
            }

            foreach(KeyValuePair<string, object> cimSwitchPair in cimSwitches)
            {
                FTN.Switch cimSwitch = cimSwitchPair.Value as FTN.Switch;

                ResourceDescription rd = CreateSwitchResourceDescription(cimSwitch);

                if (rd != null)
                {
                    delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                    report.Report.Append("Switch ID = ").Append(cimSwitch.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                }
                else
                {
                    report.Report.Append("Switch ID = ").Append(cimSwitch.ID).AppendLine(" FAILED to be converted");
                }
            }
            report.Report.AppendLine();
        }

        private ResourceDescription CreateSwitchResourceDescription(FTN.Switch cimSwitch)
        {
            ResourceDescription rd = null;
            if (cimSwitch != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.BASEVOLTAGE, importHelper.CheckOutIndexForDMSType(DMSType.BASEVOLTAGE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimSwitch.ID, gid);

                ////populate ResourceDescription
                //PowerTransformerConverter.PopulateBaseVoltageProperties(cimSwitch, rd);
            }
            return rd;
        }

        #endregion Import

    }
}
