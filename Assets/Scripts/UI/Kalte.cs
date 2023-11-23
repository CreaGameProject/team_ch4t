using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics.Eventing.Reader;
using Unity.VisualScripting;

public class Kalte : MonoBehaviour
{
    [SerializeField]
    private float animationTime = 0.3f;
    [SerializeField]
    private float windowScaleRatio = 2;
    [SerializeField]
    private GameObject[] secretText = new GameObject[3];
    private RectTransform rt;
    private Vector2 defaultWindowsSize;
    private Vector2 defaultWindowsPosition;
    private Quaternion defaultWindowsRotation;
    private bool isExpand;
    private int currentSecretNum = 1;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        defaultWindowsSize = rt.localScale;
        defaultWindowsPosition = rt.position;
        defaultWindowsRotation = rt.rotation;

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
            isExpand = false;
        }
    }

    /// <summary>
    /// ŽŸ‚Ì”é–§‚ð•\Ž¦‚·‚é
    /// </summary>
    public void AddNextSecretText()
    {
        if (currentSecretNum == 3)
        {
            Debug.Log("”é–§‚Í3‚Â‚µ‚©‚ ‚è‚Ü‚¹‚ñ");
            return;
        }
        currentSecretNum++;
        secretText[currentSecretNum].SetActive(true);
    }
}