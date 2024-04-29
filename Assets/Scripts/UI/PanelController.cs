using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [Header("パネルの開閉時間")]
    public float animationTime = 0.3f;
    [Header("WIndowの背景")]
    public GameObject backPanel;
    [Header("Window")]
    public GameObject windowPanel;

    private RectTransform windowRect;
    private Vector3 defaultScale = Vector3.zero;
    private Image backImage;
    private bool isAnimate = false;

    // Start is called before the first frame update
    void Start()
    {
        windowPanel.SetActive(true);

        windowRect = windowPanel.GetComponent<RectTransform>();
        windowRect.localScale = Vector3.zero;

        backImage = backPanel.GetComponent<Image>();
        backImage.color = Color.clear;
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
        Helper.isAllowedTextClick = false;
        //StartCoroutine(FadeGackGroundPanel());
    }

    public void ClosePanel()
    {
        isAnimate = true;
        windowRect.DOScale(0, animationTime).SetEase(Ease.InBack);
        backImage.DOFade(0, animationTime).OnComplete(() => isAnimate = false);
        Helper.isAllowedTextClick = true;
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
