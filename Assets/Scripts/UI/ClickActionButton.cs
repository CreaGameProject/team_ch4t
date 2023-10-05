using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickActionButton : MonoBehaviour, IPointerClickHandler
{
    public Action OnClickAction;
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        if (OnClickAction != null)
        {
            OnClickAction();
        }
    }
}
