using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics.Eventing.Reader;
using Unity.VisualScripting;
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
    private GameObject[] secretText = new GameObject[3];
    [SerializeField]
    private RectTransform rt;
    private Vector2 defaultWindowsSize;
    private Vector2 defaultWindowsPosition;
    private Quaternion defaultWindowsRotation;
    private bool isExpand;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        defaultWindowsSize = rt.localScale;
        defaultWindowsPosition = rt.anchoredPosition;
        defaultWindowsRotation = rt.rotation;
        backGround.DOFade(0, 0.01f);
        Board.instance.OnChangeHimituNumberExecuted += OnChangeHimituNumberExecutedHandler;

        for (int i = 0; i < 3; i++)
        {
            secretText[i].SetActive(false);
        }
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
            rt.DOAnchorPos(new Vector3(0,0,0), animationTime).SetEase(Ease.InOutCirc);
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

    private void AddNextSecretText(int secret)
    {
        secretText[secret].SetActive(true);
        
    }

    private void OnChangeHimituNumberExecutedHandler(int howManyHimituDidGet)
    {
        AddNextSecretText(howManyHimituDidGet);
    }
}