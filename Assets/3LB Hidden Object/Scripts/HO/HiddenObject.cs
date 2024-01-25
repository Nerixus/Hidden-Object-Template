using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    /// <summary>
    /// Class used on any object that should be a Hidden Object.
    /// </summary>
    public class HiddenObject : MonoBehaviour
    {
        [Header("GUI Variables")]
        public string displayNameCode;
        //public Sprite displayImage; TODO

        [Header("Found Variables")]
        public bool overrideFoundFeedback = false;
        public HIDDEN_OBJECT_EXIT_MODE customFeedback;
        public float feedbackDuration = .5f;
        public bool useCustomParticles = false;
        public ParticleSystem foundParticles;
        //private Variables
        protected HIDDEN_OBJECT_EXIT_MODE foundFeedback;

        public delegate void HandleObjectFound(HiddenObject v_hiddenObject);
        public static HandleObjectFound OnObjectFound;

        public bool ValidateIfObjectIsFound()
        {
            return GameplayManager.Instance.CurrentLoadedLevel.IsHiddenObjectOnList(this);
        }

        public virtual void SetObjectFound()
        {
            ProcessOverrides();
            if (ValidateIfObjectIsFound())
            {
                if (useCustomParticles && foundParticles != null)
                    foundParticles.Play();
                OnObjectFound?.Invoke(this);
            }    
        }

        public virtual void SetupHiddenObjectFoundFeedback(HIDDEN_OBJECT_EXIT_MODE v_feedbackType)
        {
            foundFeedback = v_feedbackType;
        }

        public virtual void SetupHiddenObjectCollider()
        {

        }

        public virtual void ActivateHiddenObject()
        {

        }

        public void ProcessOverrides()
        {
            if (overrideFoundFeedback)
                foundFeedback = customFeedback;
        }

        public void SetInactive(float v_delay)
        {
            StartCoroutine(SetInactiveRoutine(v_delay));
        }
        private IEnumerator SetInactiveRoutine(float v_delay)
        {
            yield return new WaitForSeconds(v_delay);
            gameObject.SetActive(false);
        }
    }

    public enum HIDDEN_OBJECT_EXIT_MODE
    {
        NONE,
        DESTROY,
        ANIMATE,
        FADE
    }
}
