using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsController : MonoBehaviour
{
    private List<BaseAnimation> animations = new List<BaseAnimation>();

    void Update()
    {
        for (int i = 0; i < animations.Count; i ++)
        {
            BaseAnimation animation = animations[i];
            animation.Run();
            if (animation.HasEnded)
            {
                animations.RemoveAt(i);
            }
        }
    }

    public void AddAnimation(BaseAnimation animation)
    {
        this.animations.Add(animation);
    }
}
