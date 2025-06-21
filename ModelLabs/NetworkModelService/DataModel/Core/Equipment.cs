using FTN.Common;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
	public class Equipment : PowerSystemResource
	{		
		private bool aggregate;
        private bool normallyInService;
        private long equipmentContainer = 0;

        public Equipment(long globalId) : base(globalId)
        {
        }

        public bool Aggregate
        {
            get { return aggregate; }
            set { aggregate = value; }
        }

        public bool NormallyInService
        {
            get { return normallyInService; }
            set { normallyInService = value; }
        }

        public long EquipmentContainer
        {
            get { return equipmentContainer; }
            set { equipmentContainer = value; }
        }

        public override bool Equals(object obj)
        {
			if (base.Equals(obj))
			{
				Equipment x = (Equipment)obj;

                return (x.aggregate == this.aggregate) &&
                       (x.normallyInService == this.normallyInService) &&
                       (x.equipmentContainer == this.equipmentContainer);
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
				case ModelCode.EQUIPMENT_AGGREGATE:
				case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
				case ModelCode.EQUIPMENT_EQUIPMENTCONTAINER:
					return true;
				default:
					return base.HasProperty(property);
			}
		}

		public override void GetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.EQUIPMENT_AGGREGATE:
					property.SetValue(aggregate);
					break;

				case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
					property.SetValue(normallyInService);
					break;

                case ModelCode.EQUIPMENT_EQUIPMENTCONTAINER:
                    property.SetValue(equipmentContainer);
                    break;

                default:
					base.GetProperty(property);
					break;
			}
		}

		public override void SetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.EQUIPMENT_AGGREGATE:
                    aggregate = property.AsBool();
					break;

				case ModelCode.EQUIPMENT_NORMALLYINSERVICE:
                    normallyInService = property.AsBool();
					break;
                
				case ModelCode.EQUIPMENT_EQUIPMENTCONTAINER:
                    equipmentContainer = property.AsLong();
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
                return equipmentContainer != 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (equipmentContainer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.EQUIPMENT_EQUIPMENTCONTAINER] = new List<long>();
                references[ModelCode.EQUIPMENT_EQUIPMENTCONTAINER].Add(equipmentContainer);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        #endregion IReference implementation
    }
}
