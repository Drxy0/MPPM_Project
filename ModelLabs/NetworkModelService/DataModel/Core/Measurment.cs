using FTN.Common;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class Measurment : IdentifiedObject
    {
        private long terminal = 0;
        private long powerSystemResource = 0;

        public Measurment(long globalId) : base(globalId)
        {
        }

        public long Terminal
        {
            get { return terminal; }
            set { terminal = value; }
        }

        public long PowerSystemResource
        {
            get { return powerSystemResource; }
            set { powerSystemResource = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Measurment x = (Measurment)obj;
                return (x.terminal == this.terminal) &&
                       (x.powerSystemResource == this.powerSystemResource);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IAccess implementation

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.MEASURMENT_TERMINAL:
                case ModelCode.MEASURMENT_PSR:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.MEASURMENT_TERMINAL:
                    prop.SetValue(terminal);
                    break;

                case ModelCode.MEASURMENT_PSR:
                    prop.SetValue(powerSystemResource);
                    break;

                default:
                    base.GetProperty(prop);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.MEASURMENT_TERMINAL:
                    terminal = property.AsLong();
                    break;

                case ModelCode.MEASURMENT_PSR:
                    powerSystemResource = property.AsLong();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return terminal != 0 || powerSystemResource != 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (terminal != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.MEASURMENT_TERMINAL] = new List<long>();
                references[ModelCode.MEASURMENT_TERMINAL].Add(terminal);
            }

            if (powerSystemResource != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.MEASURMENT_PSR] = new List<long>();
                references[ModelCode.MEASURMENT_PSR].Add(powerSystemResource);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                // No list references to manage in this class
                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                // No list references to manage in this class
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        #endregion IReference implementation
    }
}