using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KalteController : MonoBehaviour
{
    [SerializeField]
    [Header("開閉時間")]
    private float openWindowAnimationTime = 0.3f;
    [SerializeField]
    [Header("拡縮時間")]
    private float expandAnimationTime = 0.21f;
    [SerializeField]
    [Header("開閉時の拡大率")]
    private float windowScaleRatio = 2;
    [SerializeField]
    [Header("ホバー時の拡大率")]
    private float expandRate = 1.1f;
    [SerializeField]
    [Header("開くボタン")]
    private GameObject windowOpener;
    [SerializeField]
    [Header("閉じるボタン")]
    private GameObject windowCloser;
    [SerializeField]
    [Header("カルテの背景")]
    private GameObject backgroundPanel;
    [SerializeField]
    [Header("カルテ")]
    private GameObject windowPanel;
    [SerializeField]
    [Header("カルテの構成要素全て")]
    private GameObject panel;

    private RectTransform windowRect;
    private Image backImage;

    private Vector2 defaultWindowsSize;
    private Vector2 defaultWindowsPosition;
    private Quaternion defaultWindowsRotation;
    private bool isExpand;

    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(true);

        windowRect = windowPanel.GetComponent<RectTransform>();

        backImage = backgroundPanel.GetComponent<Image>();
        backImage.color = Color.clear;

        // カルテを開くためのボタン
        windowOpener.GetComponent<Button>().onClick.AddListener(OpenPanel);

        // カルテを閉じるためのボタン
        windowCloser.GetComponent<Button>().onClick.AddListener(ClosePanel);
        windowCloser.SetActive(false);

        // デフォルトのカルテのTransformを保存
        defaultWindowsSize = windowRect.localScale;
        defaultWindowsPosition = windowRect.anchoredPosition;
        defaultWindowsRotation = windowRect.rotation;
        backgroundPanel.gameObject.SetActive(true);
        backgroundPanel.GetComponent<Image>().DOFade(0, 0.01f);

        // ホバーしたときのボタンのイベントをEventtrigerに登録
        EventTrigger enterTrigger = windowPanel.GetComponent<EventTrigger>();
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((eventDate) => { ExpandKalte(); });
        enterTrigger.triggers.Add(enterEntry);

        EventTrigger exitTrigger = windowPanel.GetComponent<EventTrigger>();
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((eventDate) => { ShrinkKalte(); });
        exitTrigger.triggers.Add(exitEntry);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// カルテを開く
    /// </summary>
    public void OpenPanel()
    {
        if (isExpand) return;
        windowCloser.SetActive(true);
        windowRect.DOScale(Vector3.one * windowScaleRatio, openWindowAnimationTime).SetEase(Ease.InOutCirc);
        windowRect.DOAnchorPos(Vector3.zero, openWindowAnimationTime).SetEase(Ease.InOutCirc);
        windowRect.DORotateQuaternion(Quaternion.identity, openWindowAnimationTime);
        backImage.DOFade(0.85f, openWindowAnimationTime);
        AudioManager.instance_AudioManager.PlaySE(3);
        isExpand = true;
    }

    /// <summary>
    /// カルテを閉じる
    /// </summary>
    public void ClosePanel()
    {
        if (!isExpand) return;
        windowCloser.SetActive(false);
        windowRect.DOScale(defaultWindowsSize, openWindowAnimationTime).SetEase(Ease.InOutCirc);
        windowRect.DOAnchorPos(defaultWindowsPosition, openWindowAnimationTime).SetEase(Ease.InOutCirc);
        windowRect.DORotateQuaternion(defaultWindowsRotation, openWindowAnimationTime);
        backImage.DOFade(0, openWindowAnimationTime);
        AudioManager.instance_AudioManager.PlaySE(3);
        isExpand = false;
    }

    public void ExpandKalte()
    {
        if (!isExpand) windowRect.DOScale(expandRate * defaultWindowsSize, expandAnimationTime);
    }

    public void ShrinkKalte()
    {
        if (!isExpand) windowRect.DOScale(defaultWindowsSize, expandAnimationTime);
    }
}
