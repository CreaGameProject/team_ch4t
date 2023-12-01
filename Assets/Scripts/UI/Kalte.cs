using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Kalte : MonoBehaviour
{
    [SerializeField]
    private float animationTime = 0.3f;
    [SerializeField]
    private float windowScaleRatio = 2;
    [SerializeField]
    private Image backGround;
    [SerializeField]
    private RectTransform rt;
    private Vector2 defaultWindowsSize;
    private Vector2 defaultWindowsPosition;
    private Quaternion defaultWindowsRotation;
    private Vector2 centerDifference;
    private Vector2 defaultAnchorMax;
    private Vector2 defaultAnchorMin;
    private bool isExpand;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        defaultAnchorMax = rt.anchorMax;
        defaultAnchorMin = rt.anchorMin;
        defaultWindowsSize = rt.localScale;
        defaultWindowsPosition = rt.anchoredPosition;
        defaultWindowsRotation = rt.rotation;
        backGround.DOFade(0, 0.01f);
        centerDifference = new Vector2(Screen.width / 2 - rt.anchoredPosition.x, Screen.height / 2 - rt.anchoredPosition.y);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenWindow()
    {
        if (!isExpand)
        {
            rt.DOScale(Vector3.one * windowScaleRatio, animationTime).SetEase(Ease.InOutCirc);
            rt.DOAnchorPos(Vector3.zero, animationTime).SetEase(Ease.InOutCirc);
            rt.DORotateQuaternion(Quaternion.identity, animationTime);
            backGround.DOFade(0.85f, animationTime);
            AudioManager.instance_AudioManager.PlaySE(3);
            isExpand = true;

        }
    }

    public void CloseWindow()
    {
        if (isExpand)
        {
            rt.DOScale(defaultWindowsSize, animationTime).SetEase(Ease.InOutCirc);
            rt.DOAnchorPos(defaultWindowsPosition, animationTime).SetEase(Ease.InOutCirc);
            rt.DORotateQuaternion(defaultWindowsRotation, animationTime);
            backGround.DOFade(0, animationTime);
            AudioManager.instance_AudioManager.PlaySE(3);
            isExpand = false;
        }
    }
}