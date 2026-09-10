using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class QuaternionExtensions
    {
        ///////////////////////////////////////////////////////////////////////////
        public static Quaternion Average(this IEnumerable<Quaternion> quaternions)
        {
            float weight;
            int count = 0;
            Quaternion average = Quaternion.identity;

            // For each quaternion, create a rotation which smoothly 
            // interpolates two quaternion by ratio (weight)
            foreach (Quaternion quaternion in quaternions)
            {
                weight = 1.0f / (count + 1);
                count++;
                average = Quaternion.Slerp(average, quaternion, weight);
            }

            return average;
        }

    }
}