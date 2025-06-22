using FTN.Common;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class PowerSystemResource : IdentifiedObject
    {
        private List<long> measurements = new List<long>();

        public PowerSystemResource(long globalId) : base(globalId)
        {
        }

        public List<long> Measurements
        {
            get { return measurements; }
            set { measurements = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                PowerSystemResource x = (PowerSystemResource)obj;
                return CompareHelper.CompareLists(x.measurements, this.measurements);
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
                case ModelCode.PSR_MEASUREMENTS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.PSR_MEASUREMENTS:
                    prop.SetValue(measurements);
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
                // No properties to set for lists (they are handled via references)
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
                return measurements.Count > 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (measurements != null && measurements.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.PSR_MEASUREMENTS] = measurements.GetRange(0, measurements.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.PSR_MEASUREMENTS:
                    measurements.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.PSR_MEASUREMENTS:
                    if (measurements.Contains(globalId))
                    {
                        measurements.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    }
                    break;

                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        #endregion IReference implementation
    }
}