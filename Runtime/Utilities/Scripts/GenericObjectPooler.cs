using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    /// <summary>
    /// Static class that helps in managing Object Pooling.
    /// </summary>
    public static class GenericObjectPooler
    {
        #region Populate

        /// <summary>
        /// Initializes a referenced list with a certain amount of instantiated objects.
        /// </summary>
        /// <typeparam name="T">Type of the elements. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsList">Referenced list that will be initialized.</param>
        /// <param name="objectToPool">Object that would be cloned and added into the list.</param>
        /// <param name="amountToPool">Quantity of instances to add to the list.</param>
        /// <param name="parentTransform">Transform used as the parent of the new instances.</param>
        public static void PopulateObjectPooler<T>(ref List<T> pooledObjectsList, T objectToPool, int amountToPool, Transform parentTransform = null) where T : MonoBehaviour
        {
            // List reset.
            pooledObjectsList = new List<T>();

            // Loop that fills the list.
            for (int i = 0; i < amountToPool; i++)
            {
                AddNewElement(ref pooledObjectsList, objectToPool, parentTransform);
            }
        }
        /// <summary>
        /// Initializes a referenced list with a certain amount of instantiated objects.
        /// </summary>
        /// <typeparam name="T">Type of the elements. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsList">Referenced list that will be initialized.</param>
        /// <param name="objectsToPoolRandom">List of Objects that would be randomly cloned and added into the list.</param>
        /// <param name="amountToPool">Quantity of instances to add to the list.</param>
        /// <param name="parentTransform">Transform used as the parent of the new instances.</param>
        public static void PopulateObjectPooler<T>(ref List<T> pooledObjectsList, List<T> objectsToPoolRandom, int amountToPool, Transform parentTransform = null) where T : MonoBehaviour
        {
            // List reset.
            pooledObjectsList = new List<T>();

            // Loop that fills the list.
            for (int i = 0; i < amountToPool; i++)
            {
                // In each loop, a new instance of a random object from the list of objects to pool is created.
                AddNewElement(ref pooledObjectsList, objectsToPoolRandom[Random.Range(0, objectsToPoolRandom.Count)], parentTransform);
            }
        }

        /// <summary>
        /// Initializes a referenced queue with a certain amount of instantiated objects.
        /// </summary>
        /// <typeparam name="T">Type of the elements. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsQueue">Referenced queue that will be initialized.</param>
        /// <param name="objectToPool">Object that would be cloned and added into the queue.</param>
        /// <param name="amountToPool">Quantity of instances to add to the queue.</param>
        /// <param name="parentTransform">Transform used as the parent of the new instances.</param>
        public static void PopulateObjectPooler<T>(ref Queue<T> pooledObjectsQueue, T objectToPool, int amountToPool, Transform parentTransform = null) where T : MonoBehaviour
        {
            // Queue reset.
            pooledObjectsQueue = new Queue<T>();

            // Loop that fills the queue.
            for (int i = 0; i < amountToPool; i++)
            {
                AddNewElement(ref pooledObjectsQueue, objectToPool, parentTransform);
            }
        }
        /// <summary>
        /// Initializes a referenced queue with a certain amount of instantiated objects.
        /// </summary>
        /// <typeparam name="T">Type of the elements. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsQueue">Referenced queue that will be initialized.</param>
        /// <param name="objectsToPoolRandom">List of Objects that would be randomly cloned and added into the queue.</param>
        /// <param name="amountToPool">Quantity of instances to add to the queue.</param>
        /// <param name="parentTransform">Transform used as the parent of the new instances.</param>
        public static void PopulateObjectPooler<T>(ref Queue<T> pooledObjectsQueue, List<T> objectsToPoolRandom, int amountToPool, Transform parentTransform = null) where T : MonoBehaviour
        {
            // Queue reset.
            pooledObjectsQueue = new Queue<T>();

            // Loop that fills the queue.
            for (int i = 0; i < amountToPool; i++)
            {
                // In each loop, a new instance of a random object from the list of objects to pool is created.
                AddNewElement(ref pooledObjectsQueue, objectsToPoolRandom[Random.Range(0, objectsToPoolRandom.Count)], parentTransform);
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// Returns the first available instance from the object pool.
        /// If all the instances are active, nothing is returned.
        /// [Warning] The GameObject must be activated in order to use the instance!
        /// </summary>
        /// <typeparam name="T">Type of the element. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsList">Referenced list that contains the instances.</param>
        /// <returns>The first available instance from the object pool, "null" if all the instances are in use.</returns>
        public static T GetPooledObject<T>(ref List<T> pooledObjectsList) where T : MonoBehaviour
        {
            return GetPooledObject(ref pooledObjectsList, null, null);
        }
        /// <summary>
        /// Returns the first available instance from the object pool.
        /// If all the instances are active, a new one is created.
        /// [Warning] The GameObject must be activated in order to use the instance!
        /// </summary>
        /// <typeparam name="T">Type of the element. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsList">Referenced list that contains the instances.</param>
        /// <param name="objectToPool">Object that would be cloned and added into the list.</param>
        /// <param name="parentTransform">Transform used as the parent of the new instances.</param>
        /// <returns>The first available instance from the object pool, a new one if all the instances are in use.</returns>
        public static T GetPooledObject<T>(ref List<T> pooledObjectsList, T objectToPool, Transform parentTransform = null) where T : MonoBehaviour
        {
            // Returns the first object of the list that is not active.
            for (int i = 0; i < pooledObjectsList.Count; i++)
            {
                if (!pooledObjectsList[i].gameObject.activeInHierarchy)
                {
                    return pooledObjectsList[i];
                }
            }

            // If all the objects are active, then there is a check if we want to
            // spawn an extra instance or not.
            if (objectToPool)
            {
                return AddNewElement(ref pooledObjectsList, objectToPool, parentTransform);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the first available instance from the object pool.
        /// If all the instances are active, nothing is returned.
        /// [Warning] The GameObject must be activated in order to use the instance!
        /// </summary>
        /// <typeparam name="T">Type of the element. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsQueue">Referenced queue that contains the instances.</param>
        /// <returns>The first instance from the object pool.</returns>
        public static T GetPooledObject<T>(ref Queue<T> pooledObjectsQueue) where T : MonoBehaviour
        {
            return GetPooledObject(ref pooledObjectsQueue, null, null);
        }
        /// <summary>
        /// Returns the first available instance from the object pool.
        /// If all the instances are active, a new one is created.
        /// [Warning] The GameObject must be activated in order to use the instance!
        /// </summary>
        /// <typeparam name="T">Type of the element. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsQueue">Referenced queue that contains the instances.</param>
        /// <param name="objectToPool">Object that would be cloned and added into the queue.</param>
        /// <param name="parentTransform">Transform used as the parent of the new instances.</param>
        /// <returns>The first available instance from the object pool, a new one if the first one is in use.</returns>
        public static T GetPooledObject<T>(ref Queue<T> pooledObjectsQueue, T objectToPool, Transform parentTransform = null) where T : MonoBehaviour
        {
            // Returns the first object of the queue, if it is not active.
            if (pooledObjectsQueue.Count > 0 && !pooledObjectsQueue.Peek().gameObject.activeInHierarchy)
            {
                T retVal = pooledObjectsQueue.Dequeue();
                pooledObjectsQueue.Enqueue(retVal);
                return retVal;
            }

            // If the object is active, then there is a check if we want to
            // spawn an extra instance or not.
            // If not, in this case we will pick the first element even if it is still in use.
            // If no elements in the queue and no prefab to spawn, then null.
            if (objectToPool)
            {
                return AddNewElement(ref pooledObjectsQueue, objectToPool, parentTransform);
            }
            else if (pooledObjectsQueue.Count > 0)
            {
                T retVal = pooledObjectsQueue.Dequeue();
                retVal.gameObject.SetActive(false);
                pooledObjectsQueue.Enqueue(retVal);
                return retVal;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds a new instance of object to pool in the referenced list.
        /// The new GameObject is deactivated by default.
        /// </summary>
        /// <typeparam name="T">Type of the element. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsList">Referenced list that will be filled with the new instance.</param>
        /// <param name="objectToPool">Object that would be cloned and added into the list.</param>
        /// <param name="parentTransform">Transform used as the parent of the new instances.</param>
        /// <returns>The new instance added into the list and deactivated.</returns>
        private static T AddNewElement<T>(ref List<T> pooledObjectsList, T objectToPool, Transform parentTransform) where T : MonoBehaviour
        {
            T obj = Object.Instantiate(objectToPool, parentTransform);
            obj.gameObject.SetActive(false);
            pooledObjectsList.Add(obj);
            return obj;
        }

        /// <summary>
        /// Adds a new instance of object to pool in the referenced queue.
        /// The new GameObject is deactivated by default.
        /// </summary>
        /// <typeparam name="T">Type of the element. Inherits from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="pooledObjectsQueue">Referenced queue that will be filled with the new instance.</param>
        /// <param name="objectToPool">Object that would be cloned and added into the queue.</param>
        /// <param name="parentTransform">Transform used as the parent of the new instances.</param>
        /// <returns>The new instance added into the queue and deactivated.</returns>
        private static T AddNewElement<T>(ref Queue<T> pooledObjectsQueue, T objectToPool, Transform parentTransform) where T : MonoBehaviour
        {
            T obj = Object.Instantiate(objectToPool, parentTransform);
            obj.gameObject.SetActive(false);
            pooledObjectsQueue.Enqueue(obj);
            return obj;
        }

        #endregion
    }
}
