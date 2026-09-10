using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class ComponentExtensions
    {
        public static T GetComponentInactive<T>(this Component component) where T : Component
        {
            T componentToGet = component.GetComponentInChildren<T>(true);
            if (componentToGet != null && componentToGet.gameObject == component.gameObject)
            {
                return componentToGet;
            }
            return null;
        }

        public static IEnumerable<T> GetComponentsInactive<T>(this Component component) where T : Component
        {
            T[] componentToGet = component.GetComponentsInChildren<T>(true);

            return componentToGet.Where((x) => x.gameObject == component.gameObject);
        }
    }
}
