using System;
using System.Collections;

namespace SPACS.Utilities
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
