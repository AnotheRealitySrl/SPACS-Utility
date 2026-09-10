using System.Collections.Generic;
using Unity.VisualScripting;

namespace Virtuademy.SDK.Core.VisualScripting
{

    public abstract class InstanceDataEventUnit<UnitOutput, DataType> : EventUnit<UnitOutput>
    {
        protected override bool register => true;

        protected Dictionary<GraphReference, DataType> instanceData = new();

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
            instanceData[instance] = GetData(instance);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            base.Uninstantiate(instance);
            if (instanceData.ContainsKey(instance))
            {
                instanceData.Remove(instance);
            }
        }

        protected abstract DataType GetData(GraphReference instance);
    }
}