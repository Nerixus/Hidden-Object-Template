using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ThreeLittleBerkana
{
    public class RaycastHandler : MonoBehaviour
    {
        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        public void CastRay(Vector2 touchPosition, Camera controllerCamera)
        {
            if (GameplayManager.Instance.GameType == GAME_TYPE.THREE_D)
            {
                RaycastHit hit;
                Ray ray = controllerCamera.ScreenPointToRay(touchPosition);

                if (Physics.Raycast(ray, out hit))
                    ProcessRaycast3D(hit);
            }
            else if (GameplayManager.Instance.GameType == GAME_TYPE.TWO_D_SPRITE)
            {
                RaycastHit2D[] hit = Physics2D.RaycastAll(controllerCamera.ScreenToWorldPoint(touchPosition), Vector2.zero);
                ProcessRaycast2D(hit);
            }
            else
            {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = touchPosition;
                raycastResults.Clear();
                EventSystem.current.RaycastAll(pointerEventData, raycastResults);
                ProcessRaycastUI();
            }
        }

        private void ProcessRaycast3D(RaycastHit hit)
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
                else
                {

                }
            }
        }

        private void ProcessRaycast2D(RaycastHit2D[] hit)
        {
            if (IsPointerOverUIObject())
                return;
            foreach (RaycastHit2D RH2D in hit)
            {
                if (RH2D.transform != null)
                {
                    ITouchableObject clickableComponent = RH2D.transform.GetComponent<ITouchableObject>();
                    if (clickableComponent != null)
                        clickableComponent.OnObjectTouched();
                }
            }
        }

        private void ProcessRaycastUI()
        {
            foreach (RaycastResult RR in raycastResults)
            {
                Debug.Log(RR.gameObject.name);
                ITouchableObject clickableComponent = RR.gameObject.GetComponent<ITouchableObject>();
                if (clickableComponent != null)
                    clickableComponent.OnObjectTouched();
            }
        }

        private bool IsPointerOverUIObject()
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
