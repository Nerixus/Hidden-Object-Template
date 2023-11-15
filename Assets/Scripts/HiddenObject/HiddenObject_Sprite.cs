using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class HiddenObject_Sprite : HiddenObject, ITouchableObject
    {
        public bool useCustomParticles = false;
        public ParticleSystem foundParticles;
        public void OnObjectTouched()
        {
            Debug.Log("I touched a 2D Object");
            Tweening.Instance.DoHiddenObjectTransformShrink(transform, 0.5f);
            if (useCustomParticles)
                foundParticles.Play();
            Destroy(gameObject, 0.5f);
        }
    }
}
