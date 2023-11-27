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
        defaultWindowsPosition = rt.position;
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
            rt.DOScale(Vector2.one * windowScaleRatio, animationTime).SetEase(Ease.InOutCirc);
            rt.DOMove(new Vector2(Screen.width / 2, Screen.height / 2), animationTime).SetEase(Ease.InOutCirc);
            rt.DORotateQuaternion(Quaternion.identity, animationTime);
            backGround.DOFade(0.85f, animationTime);
            isExpand = true;
        }
    }

    public void CloseWindow()
    {
        if (isExpand)
        {
            rt.DOScale(defaultWindowsSize, animationTime).SetEase(Ease.InOutCirc);
            rt.DOMove(defaultWindowsPosition, animationTime).SetEase(Ease.InOutCirc);
            rt.DORotateQuaternion(defaultWindowsRotation, animationTime);
            backGround.DOFade(0, animationTime);
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