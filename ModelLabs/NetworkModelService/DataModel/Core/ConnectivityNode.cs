using FTN.Common;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class ConnectivityNode : IdentifiedObject
    {
        private long topologicalNode = 0;                   // 0 9 reference
        private List<long> terminals = new List<long>();    // 1 9 reference
        private long connectivityNodeContainer = 0;         // 0 9 reference
        public ConnectivityNode(long globalId) : base(globalId)
        {
        }
        public long TopologicalNode
        {
            get { return topologicalNode; }
            set { topologicalNode = value; }
        }

        public List<long> Terminals
        {
            get { return terminals; }
            set { terminals = value; }
        }

        public long ConnectivityNodeContainer
        {
            get { return connectivityNodeContainer; }
            set { connectivityNodeContainer = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ConnectivityNode x = (ConnectivityNode)obj;
                return ((x.TopologicalNode == this.TopologicalNode) &&
                        CompareHelper.CompareLists(x.Terminals, this.Terminals) &&
                        (x.connectivityNodeContainer == this.connectivityNodeContainer));
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
                case ModelCode.CONNECTIVITYNODE_TOPOLOGICALNODE:
                case ModelCode.CONNECTIVITYNODE_TERMINALS:
                case ModelCode.CONNECTIVITYNODE_CNC:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.CONNECTIVITYNODE_TOPOLOGICALNODE:
                    prop.SetValue(topologicalNode);
                    break;

                case ModelCode.CONNECTIVITYNODE_TERMINALS:
                    prop.SetValue(terminals);
                    break;

                case ModelCode.CONNECTIVITYNODE_CNC:
                    prop.SetValue(connectivityNodeContainer);
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
                case ModelCode.CONNECTIVITYNODE_TOPOLOGICALNODE:
                    topologicalNode = property.AsReference();
                    break;

                case ModelCode.CONNECTIVITYNODE_CNC:
                    topologicalNode = property.AsReference();
                    break;

                // dont add cases for Lists

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
                return terminals.Count > 0 || base.IsReferenced;
            }
        }
        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            // long
            if (topologicalNode != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTIVITYNODE_TOPOLOGICALNODE] = new List<long>();
                references[ModelCode.CONNECTIVITYNODE_TOPOLOGICALNODE].Add(topologicalNode);
            }

            // long
            if (connectivityNodeContainer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTIVITYNODE_CNC] = new List<long>();
                references[ModelCode.CONNECTIVITYNODE_CNC].Add(connectivityNodeContainer);
            }

            // List
            if (terminals != null && terminals.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONDEQ_TERMINALS] = terminals.GetRange(0, terminals.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONNECTIVITYNODE:
                    terminals.Add(globalId);
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
                case ModelCode.TERMINAL_CONNECTIVITYNODE:

                    if (terminals.Contains(globalId))
                    {
                        terminals.Remove(globalId);
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
