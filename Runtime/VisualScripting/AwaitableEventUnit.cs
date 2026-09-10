using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Unity.VisualScripting;

using UnityEngine;

namespace Virtuademy.SDK.Core.VisualScripting
{
    public abstract class AwaitableEventUnit<T> : EventUnit<T>
    {
        protected override bool register => true;

        protected List<Flow> runningFlows = new List<Flow>();

        public async Task AwaitableTrigger(GraphReference reference, T args)
        {
            var flow = Flow.New(reference);

            if (!ShouldTrigger(flow, args))
            {
                flow.Dispose();
                return;
            }

            AssignArguments(flow, args);

            Run(flow);

            while (runningFlows.Contains(flow))
            {
                await Task.Yield();
            }
        }


        private void Run(Flow flow)
        {
            if (flow.enableDebug)
            {
                var editorData = flow.stack.GetElementDebugData<IUnitDebugData>(this);

                editorData.lastInvokeFrame = EditorTimeBinding.frame;
                editorData.lastInvokeTime = EditorTimeBinding.time;
            }

            if (coroutine)
            {
                runningFlows.Add(flow);
                Utilities.CoroutineRunner.Instance.StartCoroutine(TriggerState(flow));
                flow.StartCoroutine(trigger, flow.stack.GetElementData<Data>(this).activeCoroutines);
            }
            else
            {
                flow.Run(trigger);
            }
        }

        private IEnumerator TriggerState(Flow flow)
        {
            yield return new WaitUntil(() => { return flow.stack == null; });
            runningFlows.Remove(flow);
        }
    }
}