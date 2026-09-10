using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Unity.VisualScripting;

using UnityEngine;

namespace Virtuademy.SDK.Core.VisualScripting
{
    public abstract class AwaitableUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }


        private List<Flow> runningFlows = new List<Flow>();

        protected override void Definition()
        {
            InputTrigger = ControlInputCoroutine(nameof(InputTrigger), CoroutineAction);

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }

        private IEnumerator CoroutineAction(Flow flow)
        {
            runningFlows.Add(flow);

            CallAwaitableAction(flow);

            yield return new WaitUntil(() => !runningFlows.Contains(flow));

            yield return OutputTrigger;
        }


        private async void CallAwaitableAction(Flow flow)
        {
            await AwaitableAction(flow);

            runningFlows.Remove(flow);
        }

        protected abstract Task AwaitableAction(Flow flow);
    }
}