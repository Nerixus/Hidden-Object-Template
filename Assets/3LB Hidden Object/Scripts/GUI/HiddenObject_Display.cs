using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ThreeLittleBerkana
{
    public class HiddenObject_Display : MonoBehaviour
    {
        public RectTransform background;
        public TextMeshProUGUI displayNameText;
        public bool animateOnEnter;
        public CanvasGroup canvasGroup;

        private HiddenObject hiddenObjectReference;

        public delegate void HandleDisplayOff(HiddenObject_Display v_hiddenObjectDisplay);
        public static HandleDisplayOff OnDisplayOff;

        public void Activate(HiddenObject v_hiddenObject)
        {
            hiddenObjectReference = v_hiddenObject;
            background.gameObject.SetActive(true);
            StartCoroutine(LerpInHorizontalScale(0.5f));
        }

        public bool CompareHiddenObjectComponent(HiddenObject v_hiddenObject)
        {
            if (v_hiddenObject == hiddenObjectReference)
                return true;
            else
                return false;
        }

        public void SwitchDisplayedObject(HiddenObject v_hiddenObject)
        {
            hiddenObjectReference = v_hiddenObject;
            StartCoroutine(SwitchObjectRoutine(0.5f));
        }

        public void TurnOff()
        {
            StartCoroutine(CloseAnimation(.5f));
        }

        #region Animations
        IEnumerator LerpInHorizontalScale(float v_animSpeed = 1f)
        {
            if (animateOnEnter)
                yield return StartCoroutine(Tweening.Instance.RectTransformLerpInHorizontalScale(background, v_animSpeed));
            else
                yield return null;
            displayNameText.text = GameplayManager.Instance.CurrentLoadedLevel.levelDictionary[hiddenObjectReference.displayNameCode];
        }

        IEnumerator SwitchObjectRoutine(float v_animSpeed = 1f)
        {
            displayNameText.text = "";
            yield return StartCoroutine(Tweening.Instance.RectTransformShrinkHorizontal(background, v_animSpeed/2));
            displayNameText.text = GameplayManager.Instance.CurrentLoadedLevel.levelDictionary[hiddenObjectReference.displayNameCode];
            yield return StartCoroutine(Tweening.Instance.RectTransformMaximizeHorizontal(background, v_animSpeed / 2));
        }

        IEnumerator CloseAnimation(float v_animSpeed = 1f)
        {
            displayNameText.text = "";
            yield return StartCoroutine(Tweening.Instance.RectTransformShrinkHorizontal(background, v_animSpeed));
            yield return StartCoroutine(Tweening.Instance.CanvasGroupFadeOut(canvasGroup, v_animSpeed));
            OnDisplayOff?.Invoke(this);
        }
        #endregion

    }
}
