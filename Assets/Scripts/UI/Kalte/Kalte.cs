using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Kalte : MonoBehaviour
{

    // ================================================
    // このスクリプトは、いずれ消去予定
    // ================================================

    [SerializeField]
    private float openWindowAnimationTime = 0.3f;
    [SerializeField]
    private float expandAnimationTime = 0.21f;
    [SerializeField]
    private float windowScaleRatio = 2;
    [SerializeField]
    private Image backGround;
    [SerializeField]
    private RectTransform rt;
    [SerializeField]
    private float expandRate = 1.1f;

    private Vector2 defaultWindowsSize;
    private Vector2 defaultWindowsPosition;
    private Quaternion defaultWindowsRotation;
    private bool isExpand;

    // Start is called before the first frame update
    void Start()
    {
        defaultWindowsSize = rt.localScale;
        defaultWindowsPosition = rt.anchoredPosition;
        defaultWindowsRotation = rt.rotation;
        backGround.gameObject.SetActive(true);
        backGround.DOFade(0, 0.01f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenWindow()
    {
        if (!isExpand)
        {
            rt.DOScale(Vector3.one * windowScaleRatio, openWindowAnimationTime).SetEase(Ease.InOutCirc);
            rt.DOAnchorPos(Vector3.zero, openWindowAnimationTime).SetEase(Ease.InOutCirc);
            rt.DORotateQuaternion(Quaternion.identity, openWindowAnimationTime);
            backGround.DOFade(0.85f, openWindowAnimationTime);
            AudioManager.instance_AudioManager.PlaySE(3);
            isExpand = true;

        }
    }

    public void CloseWindow()
    {
        if (isExpand)
        {
            rt.DOScale(defaultWindowsSize, openWindowAnimationTime).SetEase(Ease.InOutCirc);
            rt.DOAnchorPos(defaultWindowsPosition, openWindowAnimationTime).SetEase(Ease.InOutCirc);
            rt.DORotateQuaternion(defaultWindowsRotation, openWindowAnimationTime);
            backGround.DOFade(0, openWindowAnimationTime);
            AudioManager.instance_AudioManager.PlaySE(3);
            isExpand = false;
        }
    }

    public void ExpandKalte()
    {
        if(!isExpand) rt.DOScale(expandRate * defaultWindowsSize, expandAnimationTime);
    }

    public void ShrinkKalte()
    {
        if(!isExpand) rt.DOScale(defaultWindowsSize, expandAnimationTime);
    }
}