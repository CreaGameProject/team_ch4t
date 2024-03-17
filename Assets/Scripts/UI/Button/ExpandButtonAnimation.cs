using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EventTrigger))]

public class ExpandButtonAnimation : MonoBehaviour
{
    public float animationTime = 0.21f;
    public float expandRate = 1.2f;
    private RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();

        if(GetComponent<EventTrigger>() == null) { gameObject.AddComponent<EventTrigger>(); }

        EventTrigger enterTrigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((eventDate) => { ExpandButton(); });
        enterTrigger.triggers.Add(enterEntry);

        EventTrigger exitTrigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((eventDate) => { ShrinkButton(); });
        exitTrigger.triggers.Add(exitEntry);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExpandButton()
    {
        rt.DOScale(expandRate, animationTime);
    }

    public void ShrinkButton()
    {
        rt.DOScale(1, animationTime);
    }
}
