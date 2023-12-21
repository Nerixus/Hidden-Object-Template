using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class Tweening : StaticInstance<Tweening>
    {
        public void DoHiddenObjectTransformShrink(Transform v_transform, float v_duration)
        {
            StartCoroutine(HiddenObjectShrink(v_transform, v_duration));
        }

        IEnumerator HiddenObjectShrink(Transform v_transform, float v_duration)
        {
            Vector3 initialScale = v_transform.localScale;
            float speed = 1f / v_duration;
            float current = 1f;
            float target = 0f;
            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                if(v_transform != null)
                    v_transform.localScale = initialScale * TweeningManager.Instance.hiddenObjectAnimationCurve.Evaluate(current);
                yield return null;
            }
        }
    }
}
