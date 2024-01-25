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
    public class TapEvent : UnityEvent<Vector2>
    {
    }
    public class GestureDetector : StaticInstance<GestureDetector>
    {
        public GESTURE_STATUS currentGesture;
        public float dragDeadZone;

        private Vector3 gestureStartPosition;
        private Camera camera;

        public PinchEventMobile OnPinchMobile;
        public ScrollWheelEventPC OnScrollWheelPC;
        public DragEvent OnDragEvent;
        public TapEvent OnTapGesture;
        public Vector3 lastGestureStartPosition;
        
        public void SetupCamera(Camera v_camera)
        {
            camera = v_camera;
        }

        void Update()
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touchCount >= 2 && currentGesture != GESTURE_STATUS.DRAGGING )
                {
                    currentGesture = GESTURE_STATUS.PINCHING;
                    OnPinchMobile.Invoke(Input.GetTouch(0), Input.GetTouch(1));
                }
                else if (Input.touchCount == 1 && currentGesture != GESTURE_STATUS.PINCHING)
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        gestureStartPosition = camera.ScreenToWorldPoint(Input.touches[0].position);
                        lastGestureStartPosition = new Vector3(gestureStartPosition.x, gestureStartPosition.y, 0);
                    }
                    else if (Input.touches[0].phase == TouchPhase.Moved)
                    {
                        Vector3 difference = gestureStartPosition - camera.ScreenToWorldPoint(Input.touches[0].position);
                        if (difference.magnitude > dragDeadZone)
                        {
                            currentGesture = GESTURE_STATUS.DRAGGING;
                            OnDragEvent.Invoke(difference);
                        }
                    }
                    else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)//End drag or tap
                    {
                        if (currentGesture == GESTURE_STATUS.DRAGGING) // end drag
                        {
                            currentGesture = GESTURE_STATUS.IDLE;
                        }
                        else
                        {
                            currentGesture = GESTURE_STATUS.TAP;
                            OnTapGesture.Invoke(Input.touches[0].position);
                            
                        }
                    }
                }
                else if (Input.touchCount == 0)
                {
                    currentGesture = GESTURE_STATUS.IDLE;
                }
            }
            else
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    OnScrollWheelPC.Invoke(Input.GetAxis("Mouse ScrollWheel") * 2f);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    gestureStartPosition = camera.ScreenToWorldPoint(Input.mousePosition);
                    lastGestureStartPosition = new Vector3(gestureStartPosition.x, gestureStartPosition.y, 0);
                }
                if (Input.GetMouseButton(0))
                {
                    Vector3 difference = gestureStartPosition - camera.ScreenToWorldPoint(Input.mousePosition);
                    if (difference.magnitude > dragDeadZone)
                    {
                        currentGesture = GESTURE_STATUS.DRAGGING;
                        OnDragEvent.Invoke(difference);
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 difference = gestureStartPosition - camera.ScreenToWorldPoint(Input.mousePosition);
                    if (currentGesture != GESTURE_STATUS.DRAGGING)
                    {
                        OnTapGesture.Invoke(Input.mousePosition);
                    }
                    else
                    {
                        currentGesture = GESTURE_STATUS.IDLE;
                    }
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
