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
        public DISPLAY_MODE displayMode;
        public string localizationKey;
        public Sprite silhouetteSprite;

        [Space(10), Header("Found Feedbacks")]
        public float feedbackDuration = .5f;
        public bool overrideFoundFeedback = false;
        public HIDDEN_OBJECT_EXIT_MODE customFeedback;
        public bool useCustomParticles = false;
        public GameObject customParticles;
        public Vector3 particleOrigin;
        //private Variables
        protected HIDDEN_OBJECT_EXIT_MODE foundFeedback;
        protected GameObject foundParticles;

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
                if(foundParticles != null)
                    Instantiate(foundParticles, GestureDetector.Instance.lastGestureStartPosition, foundParticles.transform.rotation);
                OnObjectFound?.Invoke(this);
            }    
        }

        public void SetupHiddenObjectFoundFeedback(HIDDEN_OBJECT_EXIT_MODE v_feedbackType)
        {
            foundFeedback = v_feedbackType;
        }

        public void SetupHiddenObjectFoundParticles(GameObject v_foundParticles)
        {
            foundParticles = v_foundParticles;
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
            if (useCustomParticles)
                foundParticles = customParticles;
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

    public enum DISPLAY_MODE
    {
        LOCALIZATION_NAME,
        SILHOUETTE
    }
}
