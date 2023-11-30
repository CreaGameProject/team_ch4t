using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [Header("ƒpƒlƒ‹‚ÌŠJ•ÂŽžŠÔ")]
    public float animationTime = 0.3f;
    [Header("WIndowŠO”wŒiObject")]
    public GameObject backPanel;
    [Header("Window”wŒiObject")]
    public GameObject windowPanel;

    private RectTransform windowRect;
    private Vector3 defaultScale = Vector3.zero;
    private RectTransform backRect;
    private Image backImage;
    private bool isAnimate = false;

    // Start is called before the first frame update
    void Start()
    {
        windowRect = windowPanel.GetComponent<RectTransform>();
        windowRect.localScale = Vector3.zero;

        backRect = backPanel.GetComponent<RectTransform>();
        backImage = backPanel.GetComponent<Image>();
        backImage.color = Color.clear;
        defaultScale = backRect.lossyScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPanel()
    {
        isAnimate = true;
        windowRect.DOScale(1, animationTime).SetEase(Ease.OutBack);
        backImage.DOFade(0.8f, animationTime).OnComplete(() => isAnimate = false);
        //StartCoroutine(FadeGackGroundPanel());
    }

    public void ClosePanel()
    {
        isAnimate = true;
        windowRect.DOScale(0, animationTime).SetEase(Ease.InBack);
        backImage.DOFade(0, animationTime).OnComplete(() => isAnimate = false);
        //StartCoroutine(FadeGackGroundPanel());
    }

    public void DontClosePanel()
    {
        windowRect.DOPunchScale(Vector3.one * 0.05f, animationTime).OnComplete(() => windowRect.DOScale(Vector3.one, 0.001f));
    }

    /*
    IEnumerator FadeGackGroundPanel()
    {
        while (true)
        {
            Vector3 lossScale = backTrans.lossyScale + Vector3.one * 0.000001f;
            Vector3 localScale = backTrans.localScale;
            backTrans.localScale = new Vector3(localScale.x / lossScale.x * defaultScale.x, localScale.y / lossScale.y * defaultScale.y, 0);
            if (!isAnimate) { break; }
            yield return null;
        }
        yield return null;
    }
    */
}
