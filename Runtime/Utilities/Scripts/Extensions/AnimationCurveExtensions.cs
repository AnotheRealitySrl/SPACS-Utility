using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="_Anim">Animator.</param>
        public static AnimationCurve GetInverseCurve(this AnimationCurve animationCurve)
        {
            // TODO: check c is strictly monotonic and Piecewise linear, log error otherwise
            var rev = new AnimationCurve();
            for (int i = 0; i < animationCurve.keys.Length; i++)
            {
                var kf = animationCurve.keys[i];
                var rkf = new Keyframe(kf.value, kf.time);

                if (kf.inTangent < 0)
                {
                    rkf.inTangent = 1 / kf.outTangent;
                    rkf.outTangent = 1 / kf.inTangent;
                }
                else
                {
                    rkf.inTangent = 1 / kf.inTangent;
                    rkf.outTangent = 1 / kf.outTangent;
                }

                if (kf.outTangent == 0)
                {
                    if (animationCurve.keys.Length > i + 1)
                    {
                        var nextKF = animationCurve.keys[i + 1];
                        var x = kf.time + ((nextKF.time - kf.time) / 8);
                        var y = animationCurve.Evaluate(x);
                        rkf.outTangent = 1 / GetAngularCoefficient(new Vector2(kf.time, kf.value),
                            new Vector2(x, y));
                    }
                }
                if (kf.inTangent == 0)
                {
                    if (i - 1 >= 0)
                    {
                        var prevKF = animationCurve.keys[i - 1];
                        var x = kf.time - ((kf.time - prevKF.time) / 8);
                        var y = animationCurve.Evaluate(x);
                        rkf.inTangent = 1 / GetAngularCoefficient(new Vector2(kf.time, kf.value),
                            new Vector2(x, y));
                    }
                }

                rev.AddKey(rkf);
            }
            return rev;
        }

        private static float GetAngularCoefficient(Vector2 pointA, Vector2 pointB)
        {
            return Mathf.Abs((pointA.y - pointB.y) / (pointA.x - pointB.x));
        }
    }
}