using System;
using Unity.VisualScripting;
using UnityEngine.Events;

namespace Virtuademy.SDK.Core.VisualScripting
{

    public abstract class DelegateEventUnit<UnitOutput, TEvent, TAction> : InstanceDataEventUnit<UnitOutput, TAction>
    {
        protected override bool register => true;

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
            var unityEvent = GetEvent(instance);
            var unityAction = instanceData[instance];
            AddListener(unityEvent, unityAction);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            var unityAction = instanceData[instance];
            RemoveListener(GetEvent(instance), unityAction);
            base.Uninstantiate(instance);
        }

        protected abstract TEvent GetEvent(GraphReference reference);

        protected abstract void AddListener(TEvent unityEvent, TAction action);

        protected abstract void RemoveListener(TEvent unityEvent, TAction action);
    }
    public abstract class UnityEventUnit<UnitOutput> : DelegateEventUnit<UnitOutput, UnityEvent, UnityAction>
    {
        protected override UnityAction GetData(GraphReference reference)
        {
            return () => Trigger(reference, GetArguments(reference));
        }

        protected override void AddListener(UnityEvent unityEvent, UnityAction action)
        {
            unityEvent.AddListener(action);
        }

        protected override void RemoveListener(UnityEvent unityEvent, UnityAction action)
        {
            unityEvent.RemoveListener(action);
        }

        protected abstract override UnityEvent GetEvent(GraphReference reference);

        protected abstract UnitOutput GetArguments(GraphReference reference);
    }
    public abstract class UnityEventUnit<UnitOutput, T> :
    DelegateEventUnit<UnitOutput, UnityEvent<T>, UnityAction<T>>
    {
        protected override UnityAction<T> GetData(GraphReference reference)
        {
            return (value) => Trigger(reference, GetArguments(reference, value));
        }

        protected override void AddListener(UnityEvent<T> unityEvent, UnityAction<T> action)
        {
            unityEvent.AddListener(action);
        }

        protected override void RemoveListener(UnityEvent<T> unityEvent, UnityAction<T> action)
        {
            unityEvent.RemoveListener(action);
        }

        protected abstract override UnityEvent<T> GetEvent(GraphReference reference);

        protected abstract UnitOutput GetArguments(GraphReference reference, T eventData);
    }
    public abstract class ActionEventUnit<UnitOutput> :
    DelegateEventUnit<UnitOutput, Action, Action>
    {
        protected override Action GetData(GraphReference reference)
        {
            return () => Trigger(reference, GetArguments(reference));
        }

        protected override void AddListener(Action unityEvent, Action action)
        {
            unityEvent += action;
        }

        protected override void RemoveListener(Action unityEvent, Action action)
        {
            unityEvent -= action;
        }

        protected abstract override Action GetEvent(GraphReference reference);

        protected abstract UnitOutput GetArguments(GraphReference reference);
    }

    public abstract class ActionEventUnit<UnitOutput, T> :
    DelegateEventUnit<UnitOutput, Action<T>, Action<T>>
    {
        protected override Action<T> GetData(GraphReference reference)
        {
            return (value) => Trigger(reference, GetArguments(reference, value));
        }

        protected override void AddListener(Action<T> unityEvent, Action<T> action)
        {
            unityEvent += action;
        }

        protected override void RemoveListener(Action<T> unityEvent, Action<T> action)
        {
            unityEvent -= action;
        }

        protected abstract override Action<T> GetEvent(GraphReference reference);

        protected abstract UnitOutput GetArguments(GraphReference reference, T eventData);
    }
}