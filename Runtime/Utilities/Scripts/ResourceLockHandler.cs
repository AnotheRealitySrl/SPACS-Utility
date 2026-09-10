using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public class ResourceLockHandler<T>
    {
        private List<T> items = new List<T>();

        public bool isLocked => items.Count() > 0;

        public void AddItem(T item)
        {
            Debug.Log("Adding lock " + item);
            items.Add(item);
            if (items.Count() == 1)
            {
                OnResourceLock();
            }
        }

        public void RemoveItem(T item)
        {
            Debug.Log("Removing item " + item);
            if (!items.Contains(item))
            {
                Debug.LogWarning("Trying to remove non existing lock " + item);
                return;
            }
            items.RemoveAt(items.LastIndexOf(item));
            if (items.Count() == 0)
            {
                OnResourceUnlock();
            }
        }

        public Action OnResourceLock;
        public Action OnResourceUnlock;
    }
}
