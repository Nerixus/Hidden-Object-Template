using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class Tweening : StaticInstance<Tweening>
    {
        public IEnumerator ObjectTransformShrink(Transform v_transform, float v_duration = -1f)
        {
            float speed = ConvertTweenSpeed(v_duration);
            Vector3 initialScale = v_transform.localScale;
            float current = 0f;
            float target = 1f;
            AnimationCurve localCurve = TweeningManager.Instance.objectShrinkAnim_Curve;

            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                if(v_transform != null)
                    v_transform.localScale = initialScale * localCurve.Evaluate(current);
                yield return null;
            }
        }

        //SPRITE RENDERER

        public IEnumerator spriteFadeOut(SpriteRenderer v_spriteRenderer, float v_duration = -1f)
        {
            float speed = ConvertTweenSpeed(v_duration);
            float current = 1f;
            float target = 0f;
            AnimationCurve localCurve = TweeningManager.Instance.spriteFadeOut_Curve;
            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                v_spriteRenderer.color = new Color(v_spriteRenderer.color.r, v_spriteRenderer.color.g, v_spriteRenderer.color.b, localCurve.Evaluate(current));
                yield return null;
            }
        }

        //UNITY UI
        public IEnumerator RectTransformLerpInHorizontalScale(RectTransform v_transform, float v_duration = -1f)
        {
            float speed = ConvertTweenSpeed(v_duration);
            Vector3 initialScale = v_transform.localScale;
            float current = 0f;
            float target = 1f;
            AnimationCurve localCurve = TweeningManager.Instance.rectTransformLerpInHorizontalScale_Curve;

            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                if (v_transform != null)
                    v_transform.localScale = new Vector3(initialScale.x * localCurve.Evaluate(current), initialScale.y, initialScale.z);
                yield return null;
            }
        }

        public IEnumerator RectTransformShrinkHorizontal(RectTransform v_transform, float v_duration = -1f)
        {
            float speed = ConvertTweenSpeed(v_duration);
            Vector3 initialScale = v_transform.localScale;
            float current = 0f;
            float target = 1f;
            AnimationCurve localCurve = TweeningManager.Instance.rectTransformShrinkHorizontal_Curve;

            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                if (v_transform != null)
                    v_transform.localScale = new Vector3(localCurve.Evaluate(current), initialScale.y, initialScale.z);
                yield return null;
            }
        }

        public IEnumerator RectTransformMaximizeHorizontal(RectTransform v_transform, float v_duration = -1f)
        {
            float speed = ConvertTweenSpeed(v_duration);
            Vector3 initialScale = v_transform.localScale;
            float current = 0f;
            float target = 1f;
            AnimationCurve localCurve = TweeningManager.Instance.rectTransformMaximizeHorizontal_Curve;

            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                if (v_transform != null)
                    v_transform.localScale = new Vector3(localCurve.Evaluate(current), initialScale.y, initialScale.z);
                yield return null;
            }
        }

        public IEnumerator RectTransformLerpOutHorizontalScale(RectTransform v_transform, float v_duration = -1f)
        {
            float speed = ConvertTweenSpeed(v_duration);
            Vector3 initialScale = v_transform.localScale;
            float current = 1f;
            float target = 0f;
            AnimationCurve localCurve = TweeningManager.Instance.rectTransformLerpInHorizontalScale_Curve;

            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                if (v_transform != null)
                    v_transform.localScale = new Vector3(initialScale.x * localCurve.Evaluate(current), initialScale.y, initialScale.z);
                yield return null;
            }
        }

        public IEnumerator RectTransformLerpOutScale(RectTransform v_transform, float v_duration = -1f)
        {
            float speed = ConvertTweenSpeed(v_duration);
            Vector3 initialScale = v_transform.localScale;
            float current = 0f;
            float target = 1f;
            AnimationCurve localCurve = TweeningManager.Instance.objectShrinkAnim_Curve;

            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                if (v_transform != null)
                    v_transform.localScale = Vector3.one * localCurve.Evaluate(current);
                yield return null;
            }
        }

        public IEnumerator CanvasGroupFadeOut(CanvasGroup v_canvasGroup, float v_duration = -1f)
        {
            float speed = ConvertTweenSpeed(v_duration);
            float current = 0f;
            float target = 1f;
            AnimationCurve localCurve = TweeningManager.Instance.canvasGroupFadeOut_Curve;

            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                if (v_canvasGroup != null)
                    v_canvasGroup.alpha = localCurve.Evaluate(current);
                yield return null;
            }
        }


        private float ConvertTweenSpeed(float v_intendedSpeed)
        {
            if (v_intendedSpeed < 0)
                v_intendedSpeed = TweeningManager.Instance.standardTweenSpeed;
            float speed = 1f / v_intendedSpeed;
            return speed;
        }
    }
}
