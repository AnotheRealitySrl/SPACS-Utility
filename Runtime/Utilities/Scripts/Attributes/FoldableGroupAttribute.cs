using System;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    /// <summary>
    /// Puts the field in a foldable group that can contain one or more fields.
    /// Assign the same group name to multiple fields to get them in the same foldable group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class FoldableGroupAttribute : PropertyAttribute
    {
        public string GroupName;
        public bool Expanded;

        public FoldableGroupAttribute(string groupName, bool expanded = false)
        {
            this.GroupName = groupName;
            this.Expanded = expanded;
        }
    }
}
