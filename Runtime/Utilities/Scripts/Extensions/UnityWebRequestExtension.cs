using System;
using System.Runtime.CompilerServices;

using UnityEngine.Networking;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class UnityWebRequestExtension
    {
        public struct UnityWebRequestAwaiter : INotifyCompletion
        {
            readonly UnityWebRequestAsyncOperation reqOp;

            public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation reqOp)
            {
                this.reqOp = reqOp;
            }

            public bool IsCompleted => reqOp.isDone;

            public void OnCompleted(Action continuation)
            {
                reqOp.completed += _ => continuation();
            }

            public UnityWebRequest.Result GetResult()
            {
                return reqOp.webRequest.result;
            }
        }


        public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
        {
            return new UnityWebRequestAwaiter(reqOp);
        }
    }
}