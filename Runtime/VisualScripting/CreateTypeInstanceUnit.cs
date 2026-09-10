using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace Virtuademy.SDK.Core
{
    //[UnitTitle("Reflectis Create:" + typeof(T))]
    //[UnitSurtitle(typeof(T))]
    //[UnitShortTitle("Create")]
    //[UnitCategory("Reflectis\\Create")]
    public abstract class CreateTypeInstanceUnit<T> : Unit
    {
        private GameObject gameObject;

        [DoNotSerialize]
        public List<ValueInput> Arguments { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput NewValue { get; private set; }

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);

            gameObject = instance.gameObject;
        }

        protected override void Definition()
        {

            NewValue = ValueOutput(nameof(NewValue),
                (f) =>
                {
                    Type type = typeof(T);

                    if (type != null)
                    {
                        var typeInstance = type.Instantiate();

                        foreach (var argument in Arguments)
                        {
                            if (argument.hasValidConnection || argument.hasDefaultValue)
                            {
                                var value = f.GetConvertedValue(argument);
                                if (value != null)
                                {
                                    //Set field value
                                    type.GetRuntimeFields().FirstOrDefault(x => x.Name.Equals(argument.key))?.SetValue(typeInstance, value);
                                }
                            }
                        }
                        return (T)typeInstance;
                    }
                    else
                    {
                        Debug.LogError("Cannot find Type of " + nameof(T), gameObject);
                        return default(T);
                    }
                });


            Arguments = new List<ValueInput>();

            Type type = typeof(T);

            if (type != null)
            {
                foreach (var field in type.GetRuntimeFields())
                {

                    ValueInput argument = ValueInput(field.FieldType, field.Name);

                    if (field.FieldType.IsNullable() && !field.FieldType.IsClass)
                    {
                        argument.unit.defaultValues[field.Name] = null;
                    }
                    else
                    {
                        argument.SetDefaultValue(field.FieldType.Default());
                    }

                    Arguments.Add(argument);
                }
            }

        }
    }
}
