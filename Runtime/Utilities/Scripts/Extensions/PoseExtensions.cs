using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SPACS.Utilities
{
    public static class PoseExtensions
    {
        ///////////////////////////////////////////////////////////////////////////
        public static Matrix4x4 GetMatrix(ref this Pose pose)
        {
            return Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one);
        }

        ///////////////////////////////////////////////////////////////////////////
        public static void SetMatrix(ref this Pose pose, Matrix4x4 mtx)
        {
            pose.position = mtx.GetColumn(3);
            pose.rotation = mtx.rotation;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Transforms the provided pose into the local space of the current pose.
        /// </summary>
        public static Pose Transform(this Pose pose, Pose pose2)
        {
            Pose result = Pose.identity;
            result.SetMatrix(pose.GetMatrix().inverse * pose2.GetMatrix());
            return result;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Transforms the world pose of the provided transform into the local
        /// space of the current pose.
        /// </summary>
        public static Pose Transform(this Pose pose, Transform transform)
        {
            return pose.Transform(transform.GetPose(Space.World));
        }

        ///////////////////////////////////////////////////////////////////////////
        public static Pose Lerp(Pose pose1, Pose pose2, float t)
        {
            Pose result = Pose.identity;
            Matrix4x4 lerpMatrix = MatrixExtensions.Lerp(pose1.GetMatrix(), pose2.GetMatrix(), t);
            result.SetMatrix(lerpMatrix);
            return result;
        }

        ///////////////////////////////////////////////////////////////////////////
        public static Pose LerpUnclamped(Pose pose1, Pose pose2, float t)
        {
            Pose result = Pose.identity;
            Matrix4x4 lerpMatrix = MatrixExtensions.LerpUnclamped(pose1.GetMatrix(), pose2.GetMatrix(), t);
            result.SetMatrix(lerpMatrix);
            return result;
        }

        ///////////////////////////////////////////////////////////////////////////
        public static Pose Average(this IEnumerable<Pose> poses)
        {
            Pose average = Pose.identity;

            if (poses.Count() > 0)
            {
                average.position = poses.Select(p => p.position).Average();
                average.rotation = poses.Select(p => p.rotation).Average();
            }

            return average;
        }

    }
}