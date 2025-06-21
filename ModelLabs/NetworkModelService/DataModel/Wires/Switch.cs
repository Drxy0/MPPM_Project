using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class Switch : ConductingEquipment
    {
        private bool normalOpen;
        private float ratedCurrent;
        private bool retained;
        private long switchOnCount;
        private DateTime switchOnDate;

        public Switch(long globalId) : base(globalId)
        {
        }

        public bool NormalOpen
        {
            get { return normalOpen; }
            set { normalOpen = value; }
        }

        public float RatedCurrent
        {
            get { return ratedCurrent; }
            set { ratedCurrent = value; }
        }

        public bool Retained
        {
            get { return retained; }
            set { retained = value; }
        }

        public long SwitchOnCount
        {
            get { return switchOnCount; }
            set { switchOnCount = value; }
        }

        public DateTime SwitchOnDate
        {
            get { return switchOnDate; }
            set { switchOnDate = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Switch x = (Switch)obj;
                return (x.normalOpen == this.normalOpen) &&
                       (x.ratedCurrent == this.ratedCurrent) &&
                       (x.retained == this.retained) &&
                       (x.switchOnCount == this.switchOnCount) &&
                       (x.switchOnDate == this.switchOnDate);
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
                case ModelCode.SWITCH_NORMALOPEN:
                case ModelCode.SWITCH_RATEDCURRENT:
                case ModelCode.SWITCH_RETAINED:
                case ModelCode.SWITCH_SWITCHONCOUNT:
                case ModelCode.SWITCH_SWITCHONDATE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.SWITCH_NORMALOPEN:
                    prop.SetValue(normalOpen);
                    break;

                case ModelCode.SWITCH_RATEDCURRENT:
                    prop.SetValue(ratedCurrent);
                    break;

                case ModelCode.SWITCH_RETAINED:
                    prop.SetValue(retained);
                    break;

                case ModelCode.SWITCH_SWITCHONCOUNT:
                    prop.SetValue(switchOnCount);
                    break;

                case ModelCode.SWITCH_SWITCHONDATE:
                    prop.SetValue(switchOnDate);
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
                case ModelCode.SWITCH_NORMALOPEN:
                    normalOpen = property.AsBool();
                    break;

                case ModelCode.SWITCH_RATEDCURRENT:
                    ratedCurrent = property.AsFloat();
                    break;

                case ModelCode.SWITCH_RETAINED:
                    retained = property.AsBool();
                    break;

                case ModelCode.SWITCH_SWITCHONCOUNT:
                    switchOnCount = property.AsLong();
                    break;

                case ModelCode.SWITCH_SWITCHONDATE:
                    switchOnDate = property.AsDateTime();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation

        // Switch doesn't have any additional references beyond what ConductingEquipment has
        // So we just use the base implementations

        #endregion IReference implementation
    }
}