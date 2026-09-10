using UnityEngine.Events;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class UnityEventExtension
    {
        public static void AddListenerOnce(this UnityEvent unityEvent, UnityAction action)
        {
            UnityAction actionAndRemove = null;
            actionAndRemove = () =>
            {
                action?.Invoke();
                unityEvent.RemoveListener(actionAndRemove);
            };
            unityEvent.AddListener(actionAndRemove);
        }

        public static void AddListenerOnce<T>(this UnityEvent<T> unityEvent, UnityAction<T> action)
        {
            UnityAction<T> actionAndRemove = null;
            actionAndRemove = (T argument) =>
            {
                action?.Invoke(argument);
                unityEvent.RemoveListener(actionAndRemove);
            };
            unityEvent.AddListener(actionAndRemove);
        }

        public static void AddListenerOnce<T0, T1>(this UnityEvent<T0, T1> unityEvent, UnityAction<T0, T1> action)
        {
            UnityAction<T0, T1> actionAndRemove = null;
            actionAndRemove = (T0 argument1, T1 argument2) =>
            {
                action?.Invoke(argument1, argument2);
                unityEvent.RemoveListener(actionAndRemove);
            };
            unityEvent.AddListener(actionAndRemove);
        }
    }
}
