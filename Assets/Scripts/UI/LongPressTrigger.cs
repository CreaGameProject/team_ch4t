using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongPressTrigger :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    /// <summary> 長押しと判定する時間 </summary>
    public float IntervalSecond = 1f;

    private Action _onLongPointerDown;
    private float _executeTime;

    private void Update()
    {
        if (_executeTime > 0f && _executeTime <= Time.realtimeSinceStartup)
        {
            _onLongPointerDown();
            _executeTime = -1f;
        }
    }

    private void OnDestroy()
    {
        _onLongPointerDown = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _executeTime = Time.realtimeSinceStartup + IntervalSecond;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _executeTime = -1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _executeTime = -1f;
    }

    public void AddLongPressAction(Action action)
    {
        _onLongPointerDown = action;
    }
}
