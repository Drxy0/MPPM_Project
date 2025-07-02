using System.Collections.Generic;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class TopologicalNode : IdentifiedObject
    {
        private List<long> connectivityNodes = new List<long>();
        
        public TopologicalNode(long globalId) : base(globalId)
        {
        }

        public List<long> ConnectivityNodes
        {
            get { return connectivityNodes; }
            set { connectivityNodes = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                TopologicalNode x = (TopologicalNode)obj;
                return CompareHelper.CompareLists(x.connectivityNodes, this.connectivityNodes);
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
                case ModelCode.TOPOLOGICALNODE_CONNECTIVITYNODES:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.TOPOLOGICALNODE_CONNECTIVITYNODES:
                    prop.SetValue(connectivityNodes);
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
                // lists are handled via references, so nothing here
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
                return connectivityNodes.Count > 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (connectivityNodes != null && connectivityNodes.Count > 0 &&
                (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.TOPOLOGICALNODE_CONNECTIVITYNODES] =
                    connectivityNodes.GetRange(0, connectivityNodes.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TOPOLOGICALNODE_CONNECTIVITYNODES:
                    connectivityNodes.Add(globalId);
                    break;

                case ModelCode.CONNECTIVITYNODE_TOPOLOGICALNODE:
                    // Ignore this reference - it's handled by ConnectivityNode class
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
                case ModelCode.TOPOLOGICALNODE_CONNECTIVITYNODES:
                    if (connectivityNodes.Contains(globalId))
                    {
                        connectivityNodes.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning,
                            "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.",
                            this.GlobalId, globalId);
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