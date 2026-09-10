using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Check if the animator contains the given paramter
        /// </summary>
        /// <returns><c>true</c>, if animator has parameter, <c>false</c> otherwise.</returns>
        /// <param name="_Anim">Animator.</param>
        /// <param name="_ParamName">Parameter name.</param>
        public static bool ContainsParam(this Animator _Anim, string _ParamName)
        {
            foreach (AnimatorControllerParameter param in _Anim.parameters)
            {
                if (param.name == _ParamName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}