using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAnimation : BaseAnimation
{
    private Image canvas;
    private Vector3 startPosition;
    private Vector3 endPosition;

    public CanvasAnimation(
        Image canvas,
        Vector3 startPosition,
        Vector3 endPosition,
        float animationDuration = .2f
        ) : base(animationDuration)
    {
        this.canvas = canvas;
        this.startPosition = startPosition;
        this.endPosition = endPosition;
    }

    public override void Run()
    {
        Counter += Time.deltaTime;
        float t = Mathf.Clamp01(EaseOutCubic(Counter / AnimationDuration));
        Vector3 position = Vector3.Lerp(startPosition, endPosition, t);
        canvas.rectTransform.anchorMin = position;
        if (t == 1)
        {
            HasEnded = true;
        }
    }
}

