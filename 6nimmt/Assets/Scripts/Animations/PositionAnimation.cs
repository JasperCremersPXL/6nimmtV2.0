﻿using UnityEngine;

public class PositionAnimation : BaseAnimation
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    public GameObject GameObject { get; set; }

    public PositionAnimation(
        GameObject gameObject,
        Vector3 startPosition,
        Vector3 endPosition,
        float animationDuration = .2f
        ) : base(animationDuration)
    {
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        GameObject = gameObject;
        HasEnded = false;
    }

    public override void Run()
    {
        Counter += Time.deltaTime;
        float t = Mathf.Clamp01(EaseOutCubic(Counter / AnimationDuration));
        Vector3 position = Vector3.Lerp(startPosition, endPosition, t);
        GameObject.transform.position = position;
        if (t == 1)
        {
            HasEnded = true;
        }
    }
}
