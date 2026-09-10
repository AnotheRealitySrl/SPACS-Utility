using System;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    /// <summary>
    /// Applies an indentation space before the name of the field.
    /// Use the multiplier parameter to increased the indentation size. Negative 
    /// multiplier values will be treated as zero.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class IndentAttribute : PropertyAttribute
    {
        public int multiplier { get; private set; }

        public IndentAttribute(int multiplier = 1)
        {
            this.multiplier = multiplier;
        }
    }
}
