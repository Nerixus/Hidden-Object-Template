using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ThreeLittleBerkana
{
    public class HiddenObject_Display : MonoBehaviour
    {
        public Image background;
        public TextMeshProUGUI displayNameText;
        public HiddenObject hiddenObjectReference;
        bool isActive = false;
        bool isIdle = false;

        public void Activate(HiddenObject v_hiddenObject)
        {
            hiddenObjectReference = v_hiddenObject;
            displayNameText.text = hiddenObjectReference.displayName;
            background.gameObject.SetActive(true);
            isIdle = true;
            if (!isActive)
            {
                isActive = true;
                StartCoroutine(OnAnimation());
            }
            else
            {
                StartCoroutine(MaximizeAnimation());
            }
        }

        public void SetIdle()
        {
            isIdle = true;
            displayNameText.text = "";
            StartCoroutine(ShrinkAnimation());
        }

        public void TurnOff()
        {
            isActive = false;
            displayNameText.text = "";
            StartCoroutine(CloseAnimation());
        }

        #region Animations
        IEnumerator OnAnimation()
        {
            yield return null;
        }

        IEnumerator ShrinkAnimation()
        {
            yield return null;
        }

        IEnumerator MaximizeAnimation()
        {
            yield return null;
        }

        IEnumerator CloseAnimation()
        {
            yield return null;
        }
        #endregion

    }
}
