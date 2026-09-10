using System.Collections.Generic;
using Unity.VisualScripting;

namespace Virtuademy.SDK.Core.VisualScripting
{

    public abstract class InstanceDataUnit<DataType> : Unit
    {
        /// <summary>
        /// Access this dictionary to get the unit instance data.
        /// You can use f.stack.AsReference() in flow units to get the graph reference key 
        /// </summary>
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