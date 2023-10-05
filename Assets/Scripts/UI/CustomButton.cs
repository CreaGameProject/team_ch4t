using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LongPressTrigger))]
public class CustomButton :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    public Action OnClickAction;
    public Action OnPressAction;
    public Action OnReleaseAction;
    public Action OnLongPressAction;

    private bool _isLongPress;

    private void Awake()
    {
        var longPressTrigger = gameObject.GetComponent<LongPressTrigger>();
        longPressTrigger.AddLongPressAction(OnLongPress);
    }

    private void OnDestroy()
    {
        OnClickAction = null;
        OnPressAction = null;
        OnReleaseAction = null;
        OnLongPressAction = null;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        if (OnPressAction != null)
        {
            OnPressAction();
        }

        _isLongPress = false;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        if (!_isLongPress && OnReleaseAction != null)
        {
            OnReleaseAction();
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!enabled)
        {
            return;
        }

        if (!_isLongPress && OnClickAction != null)
        {
            OnClickAction();
        }
    }

    public virtual void OnLongPress()
    {
        if (!enabled)
        {
            return;
        }

        if (OnLongPressAction != null)
        {
            OnLongPressAction();
        }

        _isLongPress = true;
    }
}
