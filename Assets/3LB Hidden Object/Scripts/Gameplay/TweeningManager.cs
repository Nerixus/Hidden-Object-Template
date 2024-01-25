using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class TweeningManager : StaticInstance<TweeningManager>
    {
        public float standardTweenSpeed = 1f;
        [Header("Object Animations")]
        public AnimationCurve objectShrinkAnim_Curve;
        public AnimationCurve objectBeatAnim_Curve;
        [Header("Sprite Renderer")]
        public AnimationCurve spriteFadeOut_Curve;
        [Header("Object Display(UI)")]
        public AnimationCurve rectTransformLerpInHorizontalScale_Curve;
        public AnimationCurve rectTransformShrinkHorizontal_Curve;
        public AnimationCurve rectTransformMaximizeHorizontal_Curve;
        public AnimationCurve canvasGroupFadeOut_Curve;
    }
}
