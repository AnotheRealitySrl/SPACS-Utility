using System;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public class AwaitableBarrier
    {
        bool set;
        Action whenSet;

        public AwaitableBarrier()
        { }

        public Awaiter GetAwaiter()
        {
            return new Awaiter(this);
        }

        public void Set()
        {
            Debug.Assert(set == false);
            set = true;
            whenSet?.Invoke();
            whenSet = null;
        }

        public bool IsSet => set;

        public class Awaiter : System.Runtime.CompilerServices.INotifyCompletion
        {
            readonly AwaitableBarrier owner;

            public Awaiter(AwaitableBarrier owner)
            {
                this.owner = owner;
            }

            public bool IsCompleted => owner.set;

            public void OnCompleted(Action continuation)
            {
                owner.whenSet = continuation;
            }

            public void GetResult()
            { }
        }
    }
}