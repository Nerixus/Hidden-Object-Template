using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;

namespace ThreeLittleBerkana
{
    [System.Serializable]
    public class PinchEventMobile : UnityEvent<Touch, Touch>
    {
    }
    [System.Serializable]
    public class ScrollWheelEventPC : UnityEvent<float>
    {
    }
    [System.Serializable]
    public class DragEvent : UnityEvent<Vector3>
    {
    }
    [System.Serializable]
    public class TapEventMobile : UnityEvent<Vector2>
    {
    }
    [System.Serializable]
    public class TapEventPC : UnityEvent<Vector3>
    {
    }

    public class GestureDetector : MonoBehaviour
    {
        public GESTURE_STATUS gestureState;

        public PinchEventMobile OnPinchMobile;
        public ScrollWheelEventPC OnScrollWheelPC;
        public DragEvent OnDragEvent;
        public TapEventMobile OnTapMobile;
        public TapEventPC OnTapPc;
        

        public float dragDistance;
        private Vector3 dragOrigin;
        private Camera camera;

        public void SetupCamera(Camera v_camera)
        {
            camera = v_camera;
        }

        void Update()
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touchCount >= 2 && gestureState != GESTURE_STATUS.DRAGGING )
                {
                    gestureState = GESTURE_STATUS.PINCHING;
                    OnPinchMobile.Invoke(Input.GetTouch(0), Input.GetTouch(1));
                }
                else if (Input.touchCount == 1 && gestureState != GESTURE_STATUS.PINCHING)
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        dragOrigin = camera.ScreenToWorldPoint(Input.touches[0].position);
                    }
                    else if (Input.touches[0].phase == TouchPhase.Moved)
                    {
                        Vector3 difference = dragOrigin - camera.ScreenToWorldPoint(Input.touches[0].position);
                        if (difference.magnitude > dragDistance)
                        {
                            gestureState = GESTURE_STATUS.DRAGGING;
                            OnDragEvent.Invoke(difference);
                        }
                    }
                    else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)//End drag or tap
                    {
                        if (gestureState == GESTURE_STATUS.DRAGGING) // end drag
                        {
                            gestureState = GESTURE_STATUS.IDLE;
                        }
                        else
                        {
                            gestureState = GESTURE_STATUS.TAP;
                            OnTapMobile.Invoke(Input.touches[0].position);
                            
                        }
                    }
                }
                else if (Input.touchCount == 0)
                {
                    gestureState = GESTURE_STATUS.IDLE;
                }
            }
            else
            {
                if (Input.mouseScrollDelta.y != 0 && gestureState != GESTURE_STATUS.DRAGGING)
                {
                    gestureState = GESTURE_STATUS.PINCHING;
                    OnScrollWheelPC.Invoke(Input.GetAxis("Mouse ScrollWheel"));
                }
                else if (gestureState != GESTURE_STATUS.PINCHING)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        dragOrigin = camera.ScreenToWorldPoint(Input.mousePosition);
                    }
                    if (Input.GetMouseButton(0))
                    {
                        Vector3 difference = dragOrigin - camera.ScreenToWorldPoint(Input.mousePosition);
                        if (difference.magnitude > dragDistance)
                        {
                            gestureState = GESTURE_STATUS.DRAGGING;
                            OnDragEvent.Invoke(difference);
                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        Vector3 difference = dragOrigin - camera.ScreenToWorldPoint(Input.mousePosition);
                        if (gestureState != GESTURE_STATUS.DRAGGING)
                        {
                            //gestureState = GESTURE_STATUS.TAP;
                            OnTapPc.Invoke(Input.mousePosition);
                        }
                        else
                        {
                            gestureState = GESTURE_STATUS.IDLE;
                        }
                    }
                }
                else if (gestureState != GESTURE_STATUS.IDLE)
                {
                    gestureState = GESTURE_STATUS.IDLE;
                }

            }
        }
    }

    public enum GESTURE_STATUS
    {
        IDLE,
        DRAGGING,
        PINCHING,
        TAP
    }
}
