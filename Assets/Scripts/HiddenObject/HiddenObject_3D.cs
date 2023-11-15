using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class HiddenObject_3D : HiddenObject, ITouchableObject
    {
        public void OnObjectTouched()
        {
            Debug.Log("I touched a 3D Object");
            Destroy(gameObject);
        }
    }
}
