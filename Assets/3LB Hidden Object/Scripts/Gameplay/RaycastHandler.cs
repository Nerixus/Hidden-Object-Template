using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace ThreeLittleBerkana
{
    public class RaycastHandler : MonoBehaviour
    {
        public Action OnRaycastFail;
        public Action OnRaycastSuccess;

        public void ProcessRaycastPC(Vector3 mousePosition, Camera controllerCamera)
        {
            if (GameplayManager.Instance.GameType == GAME_TYPE.THREE_D)
            {
                RaycastHit hit;
                Ray ray = controllerCamera.ScreenPointToRay(mousePosition);

                if (Physics.Raycast(ray, out hit))
                    ProcessRaycast3D(hit);
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(controllerCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);
                ProcessRaycast2D(hit);
            }
        }

        public void ProcessRaycastMobile(Vector2 touchPosition, Camera controllerCamera)
        {
            if (GameplayManager.Instance.GameType == GAME_TYPE.THREE_D)
            {
                RaycastHit hit;
                Ray ray = controllerCamera.ScreenPointToRay(touchPosition);

                if (Physics.Raycast(ray, out hit))
                    ProcessRaycast3D(hit);
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(controllerCamera.ScreenToWorldPoint(touchPosition), Vector2.zero);
                ProcessRaycast2D(hit);
            }
        }
        void ProcessRaycast2D(RaycastHit2D hit)
        {
            if (GameplayManager.Instance.GameType == GAME_TYPE.TWO_D_UI_WIP)
            {
                ProcessTouchedObject(hit);
            }
            else if(GameplayManager.Instance.GameType == GAME_TYPE.TWO_D_SPRITE)
            {
                if (IsPointerOverUIObject())
                    return;
                ProcessTouchedObject(hit);
            }
        }

        void ProcessRaycast3D(RaycastHit hit)
        {
            if (IsPointerOverUIObject())
                return;
            else
            {
                if (hit.transform != null)
                {
                    ITouchableObject clickableComponent = hit.transform.GetComponent<ITouchableObject>();
                    if (clickableComponent != null)
                        clickableComponent.OnObjectTouched();
                }
            }
        }

        void ProcessTouchedObject(RaycastHit2D hit)
        {
            if (hit.transform != null)
            {
                ITouchableObject clickableComponent = hit.transform.GetComponent<ITouchableObject>();
                if (clickableComponent != null)
                    clickableComponent.OnObjectTouched();
            }
        }

        bool IsPointerOverUIObject()
        {
            if (Application.isMobilePlatform)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                    return true;
                else
                    return false;
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return true;
                else
                    return false;
            }
        }
    }
}
