using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    /// <summary>
    /// Class that rapresent a variable that can be cached. By defining how to get the variable
    /// the class handles also the variable update.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheVariable<T>
    {
        private T value;
        private DateTime lastRefresh;
        private bool isRefreshing;
        private float refreshSeconds;
        private Func<Task<T>> getFunc;
        private bool autoRefresh;
        private Action onValueChanged;

        private Coroutine autoRefreshCoroutine;

        public bool NeedsRefresh
        {
            get
            {
                return lastRefresh == DateTime.MinValue || //if the last refresh equals to min value the data has never been calculated
                    (refreshSeconds >= 0 && (DateTime.UtcNow - lastRefresh).TotalSeconds > refreshSeconds);
            }
        }

        public T CacheValue
        {
            get => value;
            set
            {
                this.value = value;
                lastRefresh = DateTime.UtcNow;
                onValueChanged?.Invoke();
            }
        }
        public Action OnValueChanged { get => onValueChanged; set => onValueChanged = value; }

        public CacheVariable(float refreshSeconds, Func<Task<T>> getFunc)
        {
            lastRefresh = DateTime.MinValue;
            this.refreshSeconds = refreshSeconds;
            this.getFunc = getFunc;
        }


        public async Task<T> GetValue()
        {
            if (isRefreshing)
            {
                while (isRefreshing)
                {
                    await Task.Yield();
                }
            }
            else
            {
                if (NeedsRefresh)
                {
                    await ForceRefresh();
                }
            }
            return CacheValue;
        }

        public async Task<T> ForceRefreshAndGetValue()
        {
            await ForceRefresh();
            return CacheValue;
        }

        public async Task ForceRefresh()
        {
            isRefreshing = true;
            CacheValue = await getFunc();
            isRefreshing = false;
        }

        public void ResetTimer()
        {
            if (!isRefreshing)
            {
                lastRefresh = DateTime.UtcNow;
            }
        }

        public void InvalidateCache()
        {
            lastRefresh = DateTime.MinValue;
        }

        public async Task EnableAutoRefresh(bool value)
        {
            if (value && !autoRefresh)
            {
                await ForceRefresh();
                autoRefreshCoroutine = CoroutineRunner.Instance.StartCoroutine(AutoRefreshCoroutine());
            }
            if (!value && autoRefresh)
            {
                CoroutineRunner.Instance.StopCoroutine(autoRefreshCoroutine);
            }
            autoRefresh = value;
        }

        private IEnumerator AutoRefreshCoroutine()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(refreshSeconds);
                _ = ForceRefresh();
            }
        }
    }

    public class CacheVariable<T, U>
    {
        Dictionary<T, CacheVariable<U>> cachesVariableCollection = new Dictionary<T, CacheVariable<U>>();

        private float refreshSeconds = -1;

        private Func<T, Task<U>> getFunc;

        private Action<T> onValueChanged;

        public Action<T> OnValueChanged { get => onValueChanged; set => onValueChanged = value; }

        public CacheVariable(float refreshSeconds, Func<T, Task<U>> getFunc)
        {
            this.refreshSeconds = refreshSeconds;
            this.getFunc = getFunc;
        }

        public async Task<U> GetValue(T input)
        {
            return await GetCacheVariable(input).GetValue();
        }

        public U GetCacheValue(T input)
        {
            return GetCacheVariable(input).CacheValue;
        }

        public void UpdateValue(T input, U value)
        {
            GetCacheVariable(input).CacheValue = value;
        }

        public async Task ForceRefresh()
        {
            foreach (var cache in cachesVariableCollection.Values)
            {
                await cache.ForceRefresh();
            }
        }

        public async Task ForceRefresh(T input)
        {
            await GetCacheVariable(input).ForceRefresh();
        }

        public void InvalidateCache()
        {
            foreach (var cache in cachesVariableCollection.Values)
            {
                cache.InvalidateCache();
            }
        }
        public void InvalidateCache(T input)
        {
            GetCacheVariable(input).InvalidateCache();
        }
        public bool NeedsRefresh(T input)
        {
            return !cachesVariableCollection.ContainsKey(input) || cachesVariableCollection[input].NeedsRefresh;
        }

        public async Task EnableAutoRefresh(T input, bool enable)
        {
            await GetCacheVariable(input).EnableAutoRefresh(enable);
        }

        private CacheVariable<U> GetCacheVariable(T input)
        {
            if (!cachesVariableCollection.ContainsKey(input))
            {
                //Add cache variable to dictionary
                Func<Task<U>> func = async () => { return await getFunc(input); };
                CacheVariable<U> cacheVariable = new CacheVariable<U>(refreshSeconds, func);
                cacheVariable.OnValueChanged += () => { onValueChanged?.Invoke(input); };
                cachesVariableCollection.Add(input, cacheVariable);
            }
            return cachesVariableCollection[input];
        }


    }
}
