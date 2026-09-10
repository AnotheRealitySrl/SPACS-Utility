using Virtuademy.SDK.Core.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public class Interpolator
    {
        private float duration;

        private AnimationCurve ease;

        private AnimationCurve inverseEase;

        private Action<float> lerpAction;

        private Func<float> getStartTimeFunction;

        private float currentTime = 0;

        private Task runningInterpolation = null;

        private bool runningForward = false;

        private bool runningBackward = false;

        public AnimationCurve InverseEase
        {
            get
            {
                if (inverseEase == null)
                {
                    inverseEase = ease.GetInverseCurve();
                }
                return inverseEase;
            }
        }

        public Interpolator(float duration, Action<float> lerpAction, Func<float> getStartTimeFunction, AnimationCurve ease = null)
        {
            this.duration = duration;
            this.lerpAction = lerpAction;
            this.ease = ease;
            if (ease == null)
            {
                this.ease = AnimationCurve.Constant(0, duration, 1);
            }

            this.getStartTimeFunction = getStartTimeFunction;
        }

        public async Task PlayForward()
        {
            Stop();
            runningInterpolation = PlayInterpolationForward();
            await runningInterpolation;
        }

        private async Task PlayInterpolationForward()
        {
            if (duration <= 0)
            {
                lerpAction(1);
                return;
            }
            currentTime = getStartTimeFunction();
            lerpAction((currentTime / duration) * ease.Evaluate(currentTime / duration));
            runningForward = true;
            while (currentTime < duration && runningForward)
            {
                currentTime = Math.Min(duration, currentTime + Time.deltaTime);
                await Task.Yield();
                lerpAction(currentTime / duration * ease.Evaluate(currentTime / duration));
            }
        }

        public async Task PlayBackwards()
        {
            Stop();
            runningInterpolation = PlayInterpolationBackwards();
            await runningInterpolation;
        }

        private async Task PlayInterpolationBackwards()
        {
            if (duration <= 0)
            {
                lerpAction(0);
                return;
            }
            currentTime = getStartTimeFunction();
            lerpAction(currentTime / duration * ease.Evaluate(currentTime / duration));
            runningBackward = true;
            while (currentTime > 0 && runningBackward)
            {
                currentTime = Math.Max(0, currentTime - Time.deltaTime);
                await Task.Yield();
                lerpAction(currentTime / duration * ease.Evaluate(currentTime / duration));
            }
        }

        public void Stop()
        {
            runningBackward = false;
            runningForward = false;
        }
    }
}
