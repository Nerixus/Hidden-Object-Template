using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class HiddenObject_3D : HiddenObject, ITouchableObject
    {
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
                        StartCoroutine(Tweening.Instance.ObjectTransformShrink(transform, feedbackDuration));
                        SetInactive(feedbackDuration);
                        break;
                    case HIDDEN_OBJECT_EXIT_MODE.FADE:
                        //StartCoroutine(Tweening.Instance.spriteFadeOut(GetComponent<SpriteRenderer>(), feedbackDuration));
                        SetInactive(feedbackDuration);
                        break;
                }
            }
        }

        public override void SetObjectFound()
        {
            base.SetObjectFound();
            if (TryGetComponent<Collider>(out Collider col))
                col.enabled = false;
        }

        public override void SetupHiddenObjectCollider()
        {
            base.SetupHiddenObjectCollider();
            if (TryGetComponent<Collider>(out Collider col))
                col.enabled = false;
        }

        public override void ActivateHiddenObject()
        {
            base.ActivateHiddenObject();
            if (TryGetComponent<Collider>(out Collider col))
                col.enabled = true;
        }
    }
}
