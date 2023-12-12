using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderSound : MonoBehaviour, IPointerUpHandler
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private ButtonSePlayer sePlayer;

    private bool isChanged = false;

    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(OnValueChanged);
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isChanged)
        {
            sePlayer.PlayHighlightedButtonSe();
            isChanged = false;
        }
    }

    private void OnValueChanged(float value)
    {
        isChanged = true;
    }
}
