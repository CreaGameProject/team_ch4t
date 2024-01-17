using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KalteController : MonoBehaviour
{
    [SerializeField]
    [Header("�J����")]
    private float openWindowAnimationTime = 0.3f;
    [SerializeField]
    [Header("�g�k����")]
    private float expandAnimationTime = 0.21f;
    [SerializeField]
    [Header("�J���̊g�嗦")]
    private float windowScaleRatio = 2;
    [SerializeField]
    [Header("�z�o�[���̊g�嗦")]
    private float expandRate = 1.1f;
    [SerializeField]
    [Header("�J���{�^��")]
    private GameObject windowOpener;
    [SerializeField]
    [Header("����{�^��")]
    private GameObject windowCloser;
    [SerializeField]
    [Header("�J���e�̔w�i")]
    private GameObject backgroundPanel;
    [SerializeField]
    [Header("�J���e")]
    private GameObject windowPanel;
    [SerializeField]
    //[Header("�J���e�̍\���v�f�S��")]
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

        // �J���e���J�����߂̃{�^��
        windowOpener.GetComponent<Button>().onClick.AddListener(OpenPanel);

        // �J���e����邽�߂̃{�^��
        windowCloser.GetComponent<Button>().onClick.AddListener(ClosePanel);
        windowCloser.SetActive(false);

        // �f�t�H���g�̃J���e��Transform��ۑ�
        defaultWindowsSize = windowRect.localScale;
        defaultWindowsPosition = windowRect.anchoredPosition;
        defaultWindowsRotation = windowRect.rotation;
        backgroundPanel.gameObject.SetActive(true);
        backgroundPanel.GetComponent<Image>().DOFade(0, 0.01f);

        // �z�o�[�����Ƃ��̃{�^���̃C�x���g��Eventtriger�ɓo�^
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
    /// �J���e���J��
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
    /// �J���e�����
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
        if (!isExpand)
        {
            windowRect.DOScale(expandRate * defaultWindowsSize, expandAnimationTime);
            AudioManager.instance_AudioManager.PlaySE(1);
        }
    }

    public void ShrinkKalte()
    {
        if (!isExpand)
        {
            windowRect.DOScale(defaultWindowsSize, expandAnimationTime);
            AudioManager.instance_AudioManager.PlaySE(1);
        }
    }
}
