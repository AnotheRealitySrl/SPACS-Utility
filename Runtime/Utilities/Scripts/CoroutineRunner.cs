using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    /// <summary>
    /// Empty classed that derives form monobehaviour.
    /// Used to add to empty gameobject to start coroutines.
    /// Unity doesn't allow to add empty Monobehaviour to objects :(
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CoroutineRunner");
                    instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
