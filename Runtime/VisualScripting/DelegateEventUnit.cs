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
    /// <summary>
    /// A node driven by a plain C# <see cref="Action"/> event rather than a <c>UnityEvent</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It cannot reuse <see cref="DelegateEventUnit{UnitOutput, TEvent, TAction}"/>'s shape, which
    /// hands the event itself to <c>AddListener</c>. That works for a <c>UnityEvent</c>, which is an
    /// object with mutable state, and cannot work for a delegate: <c>+=</c> on a parameter rebinds
    /// the local and the caller's event never hears about it. So the subclass says how to subscribe
    /// instead of handing over something to subscribe to.
    /// </para>
    /// </remarks>
    public abstract class ActionEventUnit<UnitOutput> : InstanceDataEventUnit<UnitOutput, Action>
    {
        protected override bool register => true;

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
            Subscribe(instanceData[instance]);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            Unsubscribe(instanceData[instance]);
            base.Uninstantiate(instance);
        }

        protected override Action GetData(GraphReference reference)
        {
            return () => Trigger(reference, GetArguments(reference));
        }

        /// <summary>Adds <paramref name="handler"/> to the event this node listens to.</summary>
        protected abstract void Subscribe(Action handler);

        /// <summary>Removes it again. Must undo exactly what <see cref="Subscribe"/> did.</summary>
        protected abstract void Unsubscribe(Action handler);

        protected abstract UnitOutput GetArguments(GraphReference reference);
    }

    /// <inheritdoc cref="ActionEventUnit{UnitOutput}"/>
    public abstract class ActionEventUnit<UnitOutput, T> : InstanceDataEventUnit<UnitOutput, Action<T>>
    {
        protected override bool register => true;

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
            Subscribe(instanceData[instance]);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            Unsubscribe(instanceData[instance]);
            base.Uninstantiate(instance);
        }

        protected override Action<T> GetData(GraphReference reference)
        {
            return (value) => Trigger(reference, GetArguments(reference, value));
        }

        /// <inheritdoc cref="ActionEventUnit{UnitOutput}.Subscribe"/>
        protected abstract void Subscribe(Action<T> handler);

        /// <inheritdoc cref="ActionEventUnit{UnitOutput}.Unsubscribe"/>
        protected abstract void Unsubscribe(Action<T> handler);

        protected abstract UnitOutput GetArguments(GraphReference reference, T eventData);
    }

    /// <summary>
    /// The two-value variant, for an event whose payload the node splits into separate ports.
    /// </summary>
    public abstract class ActionEventUnit<UnitOutput, T1, T2> :
    InstanceDataEventUnit<UnitOutput, Action<T1, T2>>
    {
        protected override bool register => true;

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);
            Subscribe(instanceData[instance]);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            Unsubscribe(instanceData[instance]);
            base.Uninstantiate(instance);
        }

        protected override Action<T1, T2> GetData(GraphReference reference)
        {
            return (first, second) => Trigger(reference, GetArguments(reference, first, second));
        }

        /// <inheritdoc cref="ActionEventUnit{UnitOutput}.Subscribe"/>
        protected abstract void Subscribe(Action<T1, T2> handler);

        /// <inheritdoc cref="ActionEventUnit{UnitOutput}.Unsubscribe"/>
        protected abstract void Unsubscribe(Action<T1, T2> handler);

        protected abstract UnitOutput GetArguments(GraphReference reference, T1 first, T2 second);
    }
}