using System;
using System.Collections;

namespace Virtuademy.SDK.Core.Utilities
{
    /// <summary>
    /// Extend base system Action.
    /// </summary>
    public static class ActionExtensions
    {

        /// <summary>
        /// Invoke action at next frame.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static IEnumerable InvokeNextFrame(this Action _this)
        {
            yield return null;
            _this.Invoke();
        }

    }

}
