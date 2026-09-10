using System;
using System.Collections;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    /// <summary>
    /// Extend base system Action.
    /// </summary>
    public static class MonoBehaviourExtensions
    {

        /// <summary>
        /// Invoke action at next frame.
        /// </summary>
        public static void InvokeNextFrame(this MonoBehaviour mono, Action action)
        {
            mono.StartCoroutine(CoroInvokeNextFrame(action));
        }

        private static IEnumerator CoroInvokeNextFrame(Action action)
        {
            yield return null;
            action();
        }

        /// <summary>
        /// Invoke action after some time.
        /// </summary>
        public static void InvokeAfter(this MonoBehaviour mono, float seconds, Action action)
        {
            mono.StartCoroutine(CoroInvokeAfter(seconds, action));
        }

        private static IEnumerator CoroInvokeAfter(float seconds, Action action)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action();
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go.TryGetComponent<T>(out T result))
            {
                return result;
            }
            else
            {
                return go.AddComponent<T>();
            }
        }

        /// <summary>
        /// Cloning serializable values from original one.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="original"></param>
        public static void CopyValuesFromOtherComponent(this Component _this, Component original)
        {
            var json = JsonUtility.ToJson(original);
            JsonUtility.FromJsonOverwrite(json, _this);
        }

        /// <summary>
        /// Add typed component cloning serializable values from original one.
        /// </summary>
        /// <typeparam name="T">Return component just created.</typeparam>
        /// <param name="_this">This gameobject.</param>
        /// <param name="original">Original component to clone to the new one.</param>
        public static T AddComponent<T>(this GameObject _this, T original) where T : Component
        {
            var json = JsonUtility.ToJson(original);
            T newComponent = _this.gameObject.AddComponent<T>();
            JsonUtility.FromJsonOverwrite(json, newComponent);
            return newComponent;
        }

    }

}
