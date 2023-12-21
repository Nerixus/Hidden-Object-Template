using UnityEngine;

namespace ThreeLittleBerkana
{
    public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            if (Instance != null) Destroy(gameObject);
            base.Awake();
        }
    }

}

