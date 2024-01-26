using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThreeLittleBerkana
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HiddenObject_UI : HiddenObject, ITouchableObject
    {
        RectTransform rectTransform;
        CanvasGroup canvasGroup;
        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
        public void OnObjectTouched()
        {
            if (ValidateIfObjectIsFound())
            {
                SetObjectFound();
                switch (foundFeedback)
                {
                    default:
                    case HIDDEN_OBJECT_EXIT_MODE.NONE:
                        break;
                    case HIDDEN_OBJECT_EXIT_MODE.DESTROY:
                        Destroy(gameObject, feedbackDuration);
                        break;
                    case HIDDEN_OBJECT_EXIT_MODE.SHRINK:
                        StartCoroutine(Tweening.Instance.RectTransformLerpOutScale(rectTransform, feedbackDuration));
                        SetInactive(feedbackDuration);
                        break;
                    case HIDDEN_OBJECT_EXIT_MODE.FADE:
                        StartCoroutine(Tweening.Instance.CanvasGroupFadeOut(canvasGroup, feedbackDuration));
                        SetInactive(feedbackDuration);
                        break;
                }
            }
        }
    }
}
