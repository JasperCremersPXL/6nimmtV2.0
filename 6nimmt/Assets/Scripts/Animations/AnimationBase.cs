using UnityEngine;

namespace Assets.Scripts.Animations
{
    public abstract class AnimationBase
    {
        public float AnimationDuration { get; set; }
        public float Counter { get; set; }
        public bool HasEnded { get; set; }

        protected AnimationBase(float animationDuration)
        {
            AnimationDuration = animationDuration;
            Counter = 0;
            HasEnded = false;
        }

        public float EaseOutCubic(float t)
        {
            return 1 - Mathf.Pow(1 - t, 3);
        }

        public abstract void Run();
    }
}

