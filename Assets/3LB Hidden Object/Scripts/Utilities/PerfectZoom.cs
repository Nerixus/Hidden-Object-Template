using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeLittleBerkana
{
    public class PerfectZoom : MonoBehaviour
    {
        public Camera camera;
        [SerializeField]
        private Vector2 originalResolution;
        [SerializeField]
        private SpriteRenderer sceneRenderer;
        [SerializeField]
        private float zoomStepPc, zoomStepMobile, minCamSize, maxCamSize, unZoomSpeed, dragDistance;

        private void Awake()
        {
            if (camera == null)
                camera = Camera.main;
            PerfectSizeZoom();
        }

        void PerfectSizeZoom()
        {
            float scaleFactor;
            float originalScreenRatio = (float)originalResolution.x / (float)originalResolution.y;
            float newScreenRatio = (float)Screen.width / (float)Screen.height;

            if (originalScreenRatio > newScreenRatio) //taller screen ratio
            {
                float intendedHeight = (float)Screen.width / originalScreenRatio;
                if ((float)Screen.height > intendedHeight)
                    scaleFactor = (float)Screen.height / intendedHeight;
                else
                    scaleFactor = intendedHeight / (float)Screen.height;


                float differenceInSize = originalScreenRatio / newScreenRatio;
                camera.orthographicSize = sceneRenderer.bounds.size.y / 2 * differenceInSize;
                sceneRenderer.transform.localScale = new Vector3(1, scaleFactor, 1);
                Debug.Log("Taller screen ratio");
            }
            else if (newScreenRatio >= originalScreenRatio) //wider screen ratio
            {
                float intendedWidth = (float)Screen.height * originalScreenRatio;
                if ((float)Screen.width > intendedWidth)
                    scaleFactor = (float)Screen.width / intendedWidth;
                else
                    scaleFactor = intendedWidth / (float)Screen.width;


                camera.orthographicSize = sceneRenderer.bounds.size.y / 2;
                sceneRenderer.transform.localScale = new Vector3(scaleFactor, 1, 1);
                Debug.Log("Wider screen ratio");
            }

            maxCamSize = camera.orthographicSize;
            minCamSize = camera.orthographicSize / 2f;
        }
    }

}

