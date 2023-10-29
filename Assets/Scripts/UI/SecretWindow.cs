using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SecretWindow : MonoBehaviour
{
    public float animationTime = 0.3f;
    public GameObject openingContents;
    public GameObject closingContents;
    private RectTransform rt;
    private Vector2 defaultWindoeSize;
    private Vector2 defaultWindoePosition;
    private Quaternion defaultWindoeRotation;
    private bool isExpand;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        defaultWindoeSize = rt.sizeDelta;
        defaultWindoePosition = rt.position;
        defaultWindoeRotation = rt.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenWindow()
    {
        if (!isExpand)
        {
            rt.DOSizeDelta(new Vector2(700, 500), animationTime).SetEase(Ease.InOutCirc);
            rt.DOMove(new Vector2(Screen.width / 2, Screen.height / 2), animationTime).SetEase(Ease.InOutCirc);
            rt.DORotateQuaternion(Quaternion.identity, animationTime);
            openingContents.GetComponent<CanvasGroup>().DOFade(1, animationTime);
            closingContents.GetComponent<CanvasGroup>().DOFade(0, animationTime);
            isExpand = true;
        }
        else
        {
            rt.DOSizeDelta(defaultWindoeSize, animationTime).SetEase(Ease.InOutCirc);
            rt.DOMove(defaultWindoePosition, animationTime).SetEase(Ease.InOutCirc);
            rt.DORotateQuaternion(defaultWindoeRotation, animationTime);
            openingContents.GetComponent<CanvasGroup>().DOFade(0, animationTime);
            closingContents.GetComponent<CanvasGroup>().DOFade(1, animationTime);
            isExpand = false;
        }

    }
}
