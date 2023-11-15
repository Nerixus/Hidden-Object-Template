using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;

namespace ThreeLittleBerkana
{
    [RequireComponent(typeof(RaycastHandler))]
    public class CameraController : MonoBehaviour
    {
        //[SerializeField]
        public Camera camera;
        RaycastHandler raycastHitInterpreter;
        //public ParticleSystem failParticles;
        //public ParticleSystem successParticles;
        //public ParticleSystem hoSparks;

        [SerializeField]
        private Vector2 intendedResolution;

        [SerializeField]
        private float zoomStepPc, zoomStepMobile, minCamSize, maxCamSize, unZoomSpeed, dragDistance;

        [SerializeField]
        private SpriteRenderer sceneRenderer;
        private float sceneMinX, sceneMaxX, sceneMinY, sceneMaxY;

        private Vector3 dragOrigin, cameraStart;

        bool isZoomed = false;
        bool zooming, dragging = false;

        public static Action OnFrustrumChange;
        static bool isTouchActive = true;
        public static bool wrongParticlesAvailable { private set; get; }

        public static Action OnUserBlockRequested;

        private void Awake()
        {
            if (camera == null)
                camera = Camera.main;
            raycastHitInterpreter = GetComponent<RaycastHandler>();
            sceneRenderer.size = new Vector2(intendedResolution.x / 100, intendedResolution.y / 100);
            PerfectSizeZoom();
        }

        private void OnEnable()
        {
            //Hint.OnHintActivated += UnZoom;
            //GameplayManager.OnObjectFound += PlayRightParticles;
        }

        private void OnDisable()
        {
            //Hint.OnHintActivated -= UnZoom;
            //GameplayManager.OnObjectFound -= PlayRightParticles;
        }
        void PerfectSizeZoom()
        {
            float scaleFactor;
            float originalScreenRatio = (float)intendedResolution.x / (float)intendedResolution.y;
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
                //Debug.Log("Taller screen ratio");
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
                //Debug.Log("Wider screen ratio");
            }

            maxCamSize = camera.orthographicSize;
            minCamSize = camera.orthographicSize / 2f;


            //calculate aspect ratio
            sceneMinX = sceneRenderer.transform.position.x - sceneRenderer.bounds.size.x / 2f;
            sceneMaxX = sceneRenderer.transform.position.x + sceneRenderer.bounds.size.x / 2f;

            sceneMinY = sceneRenderer.transform.position.y - sceneRenderer.bounds.size.y / 2f;
            sceneMaxY = sceneRenderer.transform.position.y + sceneRenderer.bounds.size.y / 2f;

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

        private void Update()
        {
            if (isTouchActive)
            {
                if (Application.isMobilePlatform)
                {
                    if (Input.touchCount >= 2 && !dragging)//Pinch (2 touches)
                    {
                        // if (TutorialManager.Instance.tutorialEnabled)
                        //     return;
                        MobileZoom(Input.GetTouch(0), Input.GetTouch(1));
                        zooming = true;
                        OnFrustrumChange?.Invoke();
                    }
                    else if (Input.touchCount == 1 && !zooming)//Drag && Tap (1 touch)
                    {
                        if (Input.touches[0].phase == TouchPhase.Began)
                        {
                            dragOrigin = camera.ScreenToWorldPoint(Input.touches[0].position);
                        }
                        else if (Input.touches[0].phase == TouchPhase.Moved)
                        {
                            // if (TutorialManager.Instance.tutorialEnabled)
                            //     return;
                            Vector3 difference = dragOrigin - camera.ScreenToWorldPoint(Input.touches[0].position);
                            if (difference.magnitude > dragDistance)
                            {
                                camera.transform.position = ClampCameraPosition(camera.transform.position + difference);
                                dragging = true;
                                OnFrustrumChange?.Invoke();
                            }
                        }
                        else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)//End drag or tap
                        {
                            if (dragging) // end drag
                            {
                                dragging = false;
                                OnFrustrumChange?.Invoke();
                            }
                            else
                            {
                                raycastHitInterpreter.ProcessRaycastMobile(Input.touches[0].position, camera);
                            }
                        }
                    }
                    else if (Input.touchCount == 0)
                    {
                        zooming = false;
                        dragging = false;
                    }
                }
                else
                {
                    PanCamera();

                    if (Input.mouseScrollDelta.y != 0)
                    {
                        // if (TutorialManager.Instance.tutorialEnabled)
                        //     return;
                        OnFrustrumChange?.Invoke();
                        Zoom(Input.GetAxis("Mouse ScrollWheel") * zoomStepPc);
                    }
                }
            }
        }
        void PanCamera()
        {
            //TO DO: Try to Refactor by reducing repetitive conditionals

            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = camera.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                // if (TutorialManager.Instance.tutorialEnabled)
                //     return;
                Vector3 difference = dragOrigin - camera.ScreenToWorldPoint(Input.mousePosition);
                if (difference.magnitude > dragDistance)
                {
                    dragging = true;
                    camera.transform.position = ClampCameraPosition(camera.transform.position + difference);
                    OnFrustrumChange?.Invoke();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3 difference = dragOrigin - camera.ScreenToWorldPoint(Input.mousePosition);
                if (!dragging)
                {
                    raycastHitInterpreter.ProcessRaycastPC(Input.mousePosition, camera);
                }
                else
                {
                    dragging = false;
                }
            }
        }

        public void Zoom(float zoomValue)
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

        void UnZoom(string v_objectName)
        {
            if (isZoomed)
            {
                StartCoroutine(ZoomLerpOut());
            }
        }

        IEnumerator ZoomLerpOut()
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

        public void MobileZoom(Touch touchZero, Touch touchOne)
        {
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * zoomStepMobile);
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

        //TODO move to VFX manager
        int failedTapCounter = 0;
        float tapCounterFailTime = 2f;
        float nextFailTapResetTime = 0;


        /*void PlayWrongParticles(Vector3 v_position)
        {
            if (!wrongParticlesAvailable) return;

            failParticles.transform.position = v_position;
            failParticles.Emit(1);
            for (int i = 0; i < failParticles.subEmitters.subEmittersCount; i++)
            {
                failParticles.subEmitters.GetSubEmitterSystem(i).Emit(1);
            }

            if (Time.time > nextFailTapResetTime)
            {
                failedTapCounter = 0;
            }
            failedTapCounter++;
            if (failedTapCounter == 7)
            {
                OnUserBlockRequested?.Invoke();
                Debug.Log("User Blocked");
            }
            nextFailTapResetTime = Time.time + tapCounterFailTime;
            Debug.Log(failedTapCounter);
        }*/

        /*void PlayRightParticles(PickableObjectModelController foundObject)
        {
            successParticles.transform.position = new Vector3(dragOrigin.x, dragOrigin.y, 0);
            hoSparks.transform.position = new Vector3(dragOrigin.x, dragOrigin.y, 0);
            successParticles.Emit(1);
            //hoSparks.Emit(30);
            for (int i = 0; i < successParticles.subEmitters.subEmittersCount; i++)
            {
                successParticles.subEmitters.GetSubEmitterSystem(i).Emit(1);

            }
        }*/
    }
}
