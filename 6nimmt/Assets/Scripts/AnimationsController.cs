using Assets.Scripts.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsController : MonoBehaviour
{
    private static List<AnimationBase> animations = new List<AnimationBase>();

    void Update()
    {
        for (int i = 0; i < animations.Count; i++)
        {
            animations[i].Run();
            if (animations[i].HasEnded)
            {
                animations.RemoveAt(i);
            }
        }
    }

    public static void AddAnimation(AnimationBase animation)
    {
        animations.Add(animation);
    }
}
