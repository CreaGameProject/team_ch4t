 using System;
 using UnityEngine;
 using UnityEngine.EventSystems;

 public class ReleaseActionButton : MonoBehaviour, IPointerUpHandler
 {
     public Action OnReleaseAction;
     public virtual void OnPointerUp(PointerEventData eventData)
     {
         if (!enabled)
         {
             return;
         }

         if (OnReleaseAction != null)
         {
             OnReleaseAction();
         }
     }
 }
