using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

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
