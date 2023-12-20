using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelControllerNew : MonoBehaviour
{
    [Header("パネルの開閉時間")]
    public float animationTime = 0.3f;
    [Header("Windowの周囲押下で閉じるか")]
    public bool canClose = true;
    [Header("Windowを開くボタン")]
    public List<GameObject> windowOpener = new List<GameObject>();
    [Header("Windowを閉じるボタン")]
    public List<GameObject> windowCloser = new List<GameObject>();
    [Header("Windowを閉じる背景ボタン")]
    public GameObject backWindowCloser;
    [Header("Windowの背景")]
    public GameObject backgroundPanel;
    [Header("Window")]
    public GameObject windowPanel;
    [Header("Windowの構成要素全て")]
    public GameObject panel;

    private RectTransform windowRect;
    private Vector3 defaultScale;
    private Image backImage;
    private bool isAnimate = false;

    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(true);

        windowRect = windowPanel.GetComponent<RectTransform>();
        defaultScale = windowRect.localScale;
        windowRect.localScale = Vector3.zero;

        backImage = backgroundPanel.GetComponent<Image>();
        backImage.color = Color.clear;

        // Windowを開くためのボタン
        if (windowOpener != null)
        {
            foreach(GameObject g in windowOpener) g.GetComponent<Button>().onClick.AddListener(OpenPanel);
        }

        // Windowを閉じるためのボタン
        if (windowCloser != null)
        {
            foreach (GameObject g in windowCloser) g.GetComponent<Button>().onClick.AddListener(ClosePanel);
        }

        // Windowを閉じるための背景ボタン
        backWindowCloser.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (canClose) { ClosePanel(); } else { DontClosePanel(); }
        });
        backWindowCloser.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Windowを開く
    /// </summary>
    public void OpenPanel()
    {
        backWindowCloser.SetActive(true);
        isAnimate = true;
        windowRect.DOScale(defaultScale, animationTime).SetEase(Ease.OutBack);
        backImage.DOFade(0.8f, animationTime).OnComplete(() => isAnimate = false);
    }

    /// <summary>
    /// Windowを閉じる
    /// </summary>
    public void ClosePanel()
    {
        backWindowCloser.SetActive(false);
        isAnimate = true;
        windowRect.DOScale(0, animationTime).SetEase(Ease.InBack);
        backImage.DOFade(0, animationTime).OnComplete(() => isAnimate = false);
    }

    /// <summary>
    /// Windowを閉じられないことを促す
    /// </summary>
    public void DontClosePanel()
    {
        windowRect.DOPunchScale(Vector3.one * 0.05f, animationTime).OnComplete(() => windowRect.DOScale(Vector3.one, 0.001f));
    }
}
