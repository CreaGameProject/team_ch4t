using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LongPressTrigger))]
public class CustomButton : Button
{
    public Action OnClickAction;
    public Action OnPressAction;
    public Action OnReleaseAction;
    public Action OnLongPressAction;

    private bool _isLongPress;

    protected override void Awake()
    {
        base.Awake();
        
        var longPressTrigger = gameObject.GetComponent<LongPressTrigger>();
        longPressTrigger.AddLongPressAction(OnLongPress);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        OnClickAction = null;
        OnPressAction = null;
        OnReleaseAction = null;
        OnLongPressAction = null;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        
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

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        
        if (!enabled)
        {
            return;
        }

        if (!_isLongPress && OnReleaseAction != null)
        {
            OnReleaseAction();
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        
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
