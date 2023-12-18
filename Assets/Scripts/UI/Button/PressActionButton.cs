using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PressActionButton : MonoBehaviour, IPointerDownHandler
{
    public Action onPressAction;
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        if (onPressAction != null)
        {
            onPressAction();
        }
    }
}
