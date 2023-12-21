using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;

namespace ThreeLittleBerkana
{
    [RequireComponent(typeof(RaycastHandler), typeof(GestureDetector))]
    public class CameraController : MonoBehaviour
    {
        public Camera camera;
        GestureDetector gestureDetector;
        RaycastHandler raycastHitInterpreter;
        //public ParticleSystem failParticles;
        //public ParticleSystem successParticles;
        //public ParticleSystem hoSparks;

        [SerializeField]
        private Vector2 intendedResolution;

        [SerializeField]
        public float zoomStepMobile, minCamSize, maxCamSize, unZoomSpeed;

        [SerializeField]
        private SpriteRenderer sceneRenderer;
        private float sceneMinX, sceneMaxX, sceneMinY, sceneMaxY;

        private Vector3 cameraStart;

        bool isZoomed = false;

        public static Action OnFrustrumChange;
        static bool isTouchActive = true;
        public static bool wrongParticlesAvailable { private set; get; }

        public static Action OnUserBlockRequested;

        private void Awake()
        {
            //Set Camera
            if (camera == null)
                camera = Camera.main;
            SetupOrthographicCamera();
            //Get components
            raycastHitInterpreter = GetComponent<RaycastHandler>();
            gestureDetector = GetComponent<GestureDetector>();
            //Setup components
            gestureDetector.SetupCamera(camera);
            //Setup Events
            SetupListeners();
        }

        private void SetupListeners()
        {
            gestureDetector.OnPinchMobile.AddListener(MobileZoom);
            gestureDetector.OnDragEvent.AddListener(MoveCamera);
            gestureDetector.OnTapMobile.AddListener(ProcessTapMobile);
            gestureDetector.OnScrollWheelPC.AddListener(Zoom);
            gestureDetector.OnTapPc.AddListener(ProcessTapPC);
        }

        void SetupOrthographicCamera()
        {
            sceneRenderer.size = new Vector2(intendedResolution.x / 100, intendedResolution.y / 100);
            float scaleFactor;
            float originalScreenRatio = (float)intendedResolution.x / (float)intendedResolution.y;
            float newScreenRatio = (float)Screen.width / (float)Screen.height;
            //Calculate OrthographicSize changes if screen ratio is different from original
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
            }
            maxCamSize = camera.orthographicSize;
            minCamSize = camera.orthographicSize / 2f;
            //Set Bounds
            sceneMinX = sceneRenderer.transform.position.x - sceneRenderer.bounds.size.x / 2f;
            sceneMaxX = sceneRenderer.transform.position.x + sceneRenderer.bounds.size.x / 2f;
            sceneMinY = sceneRenderer.transform.position.y - sceneRenderer.bounds.size.y / 2f;
            sceneMaxY = sceneRenderer.transform.position.y + sceneRenderer.bounds.size.y / 2f;
            //End Setup
            sceneRenderer.gameObject.SetActive(false);
            cameraStart = camera.transform.position;
        }

        public static void SetTouchStatus(bool v_value)
        {
            isTouchActive = v_value;
        }

        public static void SetTouchStatus(bool v_value, int v_delayMiliseconds)
        {
            Task.Delay(v_delayMiliseconds).ContinueWith(t => SetTouchStatus(v_value));
        }

        public static void SetWrongParticlesAvailability(bool v_particlesAreAvailable)
        {
            wrongParticlesAvailable = v_particlesAreAvailable;
        }

        #region Mobile
        private void MobileZoom(Touch touchZero, Touch touchOne)
        {
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * zoomStepMobile);
        }

        private void ProcessTapMobile(Vector2 v_tapPosition)
        {
            raycastHitInterpreter.ProcessRaycastMobile(v_tapPosition, camera);
        }
        #endregion

        #region PC
        private void ProcessTapPC(Vector3 v_tapPosition)
        {
            raycastHitInterpreter.ProcessRaycastPC(v_tapPosition, camera);
        }
        #endregion

        #region Shared Device
        private void Zoom(float zoomValue)
        {
            float newSize = camera.orthographicSize - zoomValue;
            camera.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
            if (camera.orthographicSize == maxCamSize)
            {
                isZoomed = false;
                camera.transform.position = cameraStart;
            }
            else
            {
                isZoomed = true;
                camera.transform.position = ClampCameraPosition(camera.transform.position);
            }

        }

        private void UnZoom(string v_objectName)
        {
            if (isZoomed)
            {
                StartCoroutine(ZoomLerpOut());
            }
        }

        private IEnumerator ZoomLerpOut()
        {
            isTouchActive = false;
            float current = 0f;
            float target = 1f;
            float currentZoomSize = camera.orthographicSize;
            while (current != target)
            {
                current = Mathf.MoveTowards(current, target, unZoomSpeed * Time.deltaTime);
                camera.orthographicSize = Mathf.Lerp(currentZoomSize, maxCamSize, current);
                camera.transform.position = ClampCameraPosition(camera.transform.position);
                yield return null;
            }
            isZoomed = false;
            yield return null;
            isTouchActive = true;
        }

        private void MoveCamera(Vector3 v_moveDelta)
        {
            camera.transform.position = ClampCameraPosition(camera.transform.position + v_moveDelta);
        }

        private Vector3 ClampCameraPosition(Vector3 targetPosition)
        {
            float cameraHeight = camera.orthographicSize;
            float camWidth = camera.orthographicSize * camera.aspect;

            float minX = sceneMinX + camWidth;
            float maxX = sceneMaxX - camWidth;
            float minY = sceneMinY + cameraHeight;
            float maxY = sceneMaxY - cameraHeight;

            float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
            float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

            return new Vector3(newX, newY, targetPosition.z);
        }
        #endregion
    }
}