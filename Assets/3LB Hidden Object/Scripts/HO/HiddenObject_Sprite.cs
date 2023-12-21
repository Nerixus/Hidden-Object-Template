using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class HiddenObject_Sprite : HiddenObject, ITouchableObject
    {
        public void OnObjectTouched()
        {
            if (ValidateIfObjectIsFound())
            {
                Tweening.Instance.DoHiddenObjectTransformShrink(transform, 0.5f);
                SetObjectFound();
            }
        }
    }
}
