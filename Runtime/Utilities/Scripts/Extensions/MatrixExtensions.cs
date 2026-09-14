using UnityEngine;

namespace SPACS.Utilities
{
    public static class MatrixExtensions
    {
        ///////////////////////////////////////////////////////////////////////////
        public static Matrix4x4 Lerp(Matrix4x4 mtx1, Matrix4x4 mtx2, float t)
        {
            Matrix4x4 lerp = Matrix4x4.zero;

            for (int col = 0; col < 4; col++)
                for (int row = 0; row < 4; row++)
                    lerp[row, col] = Mathf.Lerp(mtx1[row, col], mtx2[row, col], t);

            return lerp;
        }

        ///////////////////////////////////////////////////////////////////////////
        public static Matrix4x4 LerpUnclamped(Matrix4x4 mtx1, Matrix4x4 mtx2, float t)
        {
            Matrix4x4 lerp = Matrix4x4.zero;

            for (int col = 0; col < 4; col++)
                for (int row = 0; row < 4; row++)
                    lerp[row, col] = Mathf.LerpUnclamped(mtx1[row, col], mtx2[row, col], t);

            return lerp;
        }


    }
}