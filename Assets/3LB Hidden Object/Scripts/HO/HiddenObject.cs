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
        public string displayName;
        public Sprite displayImage;
        public delegate void HandleObjectFound(HiddenObject v_hiddenObject);
        public static HandleObjectFound OnObjectFound;

        public bool useCustomParticles = false;
        public ParticleSystem foundParticles;

        public bool ValidateIfObjectIsFound()
        {
            return GameplayManager.Instance.CurrentLoadedLevel.IsHiddenObjectOnList(this);
        }

        public void SetObjectFound()
        {
            if (ValidateIfObjectIsFound())
            {
                if (useCustomParticles)
                    foundParticles.Play();
                OnObjectFound?.Invoke(this);
                Destroy(gameObject, 0.5f);
            }    
        }
    }
}
