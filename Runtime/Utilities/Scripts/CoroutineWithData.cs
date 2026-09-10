using System.Collections;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public class CoroutineWithData
    {
        private readonly IEnumerator target;

        public Coroutine Coroutine { get; private set; }
        public object Result { get; private set; }

        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            this.target = target;
            Coroutine = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (target.MoveNext())
            {
                Result = target.Current;
                yield return Result;
            }
        }
    }
}
