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
    public class CameraController : StaticInstance<CameraController>
    {
        [Header("Setup")]
        public Camera camera;
        public Vector2 originResolution;
        public float zoomStepMobile, unZoomSpeed;

        [SerializeField] private float minOrthographicSize, maxOrthographicSize;
        [SerializeField] private SpriteRenderer playAreaViewport;

        private RaycastHandler raycastHitInterpreter;
        private float sceneMinX, sceneMaxX, sceneMinY, sceneMaxY;
        private Vector3 cameraStartPosition;
        private bool isZoomed = false;

        private static bool isTouchActive = true;

        private void Start()
        {
            //Set Camera
            if (camera == null)
                camera = Camera.main;
            SetupOrthographicCamera();
            //Get components
            raycastHitInterpreter = GetComponent<RaycastHandler>();
            //Setup Events
            SetupListeners();
        }

        private void SetupListeners()
        {
            GestureDetector.Instance.OnPinchMobile.AddListener(MobileZoom);
            GestureDetector.Instance.OnDragEvent.AddListener(MoveCamera);
            GestureDetector.Instance.OnTapGesture.AddListener(ProcessTapGesture);
            GestureDetector.Instance.OnScrollWheelPC.AddListener(OrthographicZoom);
        }

        private void SetupOrthographicCamera()
        {
            playAreaViewport.size = new Vector2(originResolution.x / 100, originResolution.y / 100);
            float scaleFactor;
            float originalScreenRatio = (float)originResolution.x / (float)originResolution.y;
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
                camera.orthographicSize = playAreaViewport.bounds.size.y / 2 * differenceInSize;
                playAreaViewport.transform.localScale = new Vector3(1, scaleFactor, 1);
            }
            else if (newScreenRatio >= originalScreenRatio) //wider screen ratio
            {
                float intendedWidth = (float)Screen.height * originalScreenRatio;
                if ((float)Screen.width > intendedWidth)
                    scaleFactor = (float)Screen.width / intendedWidth;
                else
                    scaleFactor = intendedWidth / (float)Screen.width;
                camera.orthographicSize = playAreaViewport.bounds.size.y / 2;
                playAreaViewport.transform.localScale = new Vector3(scaleFactor, 1, 1);
            }
            maxOrthographicSize = camera.orthographicSize;
            minOrthographicSize = camera.orthographicSize / 2f;
            //Set Bounds
            sceneMinX = playAreaViewport.transform.position.x - playAreaViewport.bounds.size.x / 2f;
            sceneMaxX = playAreaViewport.transform.position.x + playAreaViewport.bounds.size.x / 2f;
            sceneMinY = playAreaViewport.transform.position.y - playAreaViewport.bounds.size.y / 2f;
            sceneMaxY = playAreaViewport.transform.position.y + playAreaViewport.bounds.size.y / 2f;
            //End Setup
            playAreaViewport.gameObject.SetActive(false);
            cameraStartPosition = camera.transform.position;
        }

        #region Shared Device
        private void OrthographicZoom(float zoomValue)
        {
            if (GameplayManager.Instance.GameType != GAME_TYPE.TWO_D_UI)
            {
                float newSize = camera.orthographicSize - zoomValue;
                camera.orthographicSize = Mathf.Clamp(newSize, minOrthographicSize, maxOrthographicSize);
                if (camera.orthographicSize == maxOrthographicSize)
                {
                    isZoomed = false;
                    camera.transform.position = cameraStartPosition;
                }
                else
                {
                    isZoomed = true;
                    camera.transform.position = ClampCameraPositionOrthographic(camera.transform.position);
                }
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
                camera.orthographicSize = Mathf.Lerp(currentZoomSize, maxOrthographicSize, current);
                camera.transform.position = ClampCameraPositionOrthographic(camera.transform.position);
                yield return null;
            }
            isZoomed = false;
            yield return null;
            isTouchActive = true;
        }

        private void MoveCamera(Vector3 v_moveDelta)//TODO
        {
            //add method for UI and 3D
            camera.transform.position = ClampCameraPositionOrthographic(camera.transform.position + v_moveDelta);
        }

        private Vector3 ClampCameraPositionOrthographic(Vector3 targetPosition)
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

        #region Mobile
        private void MobileZoom(Touch touchZero, Touch touchOne)
        {
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            OrthographicZoom(difference * zoomStepMobile);
        }

        private void ProcessTapGesture(Vector2 v_tapPosition)
        {
            raycastHitInterpreter.CastRay(v_tapPosition, camera);
        }
        #endregion

        public static void SetTouchStatus(bool v_value)
        {
            isTouchActive = v_value;
        }

        public static void SetTouchStatus(bool v_value, int v_delayMiliseconds)
        {
            Task.Delay(v_delayMiliseconds).ContinueWith(t => SetTouchStatus(v_value));
        }
    }
}