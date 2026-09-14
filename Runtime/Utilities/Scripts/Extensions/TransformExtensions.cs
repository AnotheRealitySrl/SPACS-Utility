using UnityEngine;

namespace SPACS.Utilities
{
    public static class TransformExtensions
    {
        ///////////////////////////////////////////////////////////////////////////
        public static void Reset(this Transform trans, Space space = Space.Self)
        {
            if (space == Space.Self)
            {
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
                trans.localScale = Vector3.one;
            }
            else
            {
                trans.position = Vector3.zero;
                trans.rotation = Quaternion.identity;
                trans.localScale = trans.parent.InverseTransformVector(Vector3.one);
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        public static void SetWorldScale(this Transform transform, Vector3 scale)
        {
            transform.localScale = new Vector3(
                scale.x / transform.lossyScale.x,
                scale.y / transform.lossyScale.y,
                scale.z / transform.lossyScale.z
            );
        }

        ///////////////////////////////////////////////////////////////////////////
        public static void SetPose(this Transform trans, Pose pose, Space space = Space.Self)
        {
            if (space == Space.Self)
            {
                trans.localPosition = pose.position;
                trans.localRotation = pose.rotation;
            }
            else
            {
                trans.position = pose.position;
                trans.rotation = pose.rotation;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        public static void SetPose(this Transform trans, Pose pose, Transform reference)
        {
            trans.position = reference.TransformPoint(pose.position);
            trans.rotation = reference.TransformRotation(pose.rotation);
        }

        ///////////////////////////////////////////////////////////////////////////
        public static Pose GetPose(this Transform trans, Space space = Space.Self)
        {
            if (space == Space.Self)
                return new Pose(trans.localPosition, trans.localRotation);
            else
                return new Pose(trans.position, trans.rotation);
        }

        ///////////////////////////////////////////////////////////////////////////
        public static Pose GetPose(this Transform trans, Transform reference)
        {
            return new Pose(
                position: reference.InverseTransformPoint(trans.position),
                rotation: reference.InverseTransformRotation(trans.rotation)
            );
        }

        ///////////////////////////////////////////////////////////////////////////
        public static Matrix4x4 GetMatrix(this Transform trans, bool scale = false, Space space = Space.Self)
        {
            if (space == Space.Self)
                return Matrix4x4.TRS(trans.localPosition, trans.localRotation, scale ? trans.localScale : Vector3.one);
            else
                return Matrix4x4.TRS(trans.position, trans.rotation, scale ? trans.lossyScale : Vector3.one);
        }

        ///////////////////////////////////////////////////////////////////////////
        public static void SetMatrix(this Transform trans, Matrix4x4 mtx, bool scale = false, Space space = Space.Self)
        {
            if (space == Space.Self)
            {
                trans.localPosition = mtx.GetColumn(3);
                trans.localEulerAngles = mtx.rotation.eulerAngles;
                if (scale)
                    trans.localScale = mtx.lossyScale;
            }
            else
            {
                trans.position = mtx.GetColumn(3);
                trans.eulerAngles = mtx.rotation.eulerAngles;
                if (scale)
                    trans.SetWorldScale(mtx.lossyScale);
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary> Transforms rotation from local space to world space. </summary>
        public static Quaternion TransformRotation(this Transform trans, Quaternion quaternion)
        {
            return trans.rotation * quaternion;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary> Transforms pose from local space to world space. </summary>
        public static Pose TransformPose(this Transform trans, Pose pose)
        {
            pose.position = trans.TransformPoint(pose.position);
            pose.rotation = trans.TransformRotation(pose.rotation);
            return pose;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary> Transforms rotation from world space to local space. </summary>
        public static Quaternion InverseTransformRotation(this Transform trans, Quaternion quaternion)
        {
            return Quaternion.Inverse(trans.rotation) * quaternion;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary> Transforms pose from world space to local space. </summary>
        public static Pose InverseTransformPose(this Transform trans, Pose pose)
        {
            pose.position = trans.InverseTransformPoint(pose.position);
            pose.rotation = trans.InverseTransformRotation(pose.rotation);
            return pose;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>Set the parent of the transform. Unlike
        /// <see cref="Transform.SetParent"/>, it avoids to perform the operation
        /// in the special case where the transform have a disabled parent
        /// (in that case an error would be thrown, this method doesn't)</summary>
        /// <param name="trans">The transform</param>
        /// <param name="parent">The new parent</param>
        public static void SafeSetParent(this Transform trans, Transform parent)
        {
            if (trans.parent == null || trans.parent.gameObject.activeInHierarchy)
                trans.SetParent(parent, true);
        }


    }
}