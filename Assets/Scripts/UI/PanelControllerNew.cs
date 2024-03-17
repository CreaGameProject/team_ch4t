using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelControllerNew : MonoBehaviour
{
    [Header("�p�l���̊J����")]
    public float animationTime = 0.3f;
    [Header("Window�̎��͉����ŕ��邩")]
    public bool canClose = true;
    [Header("Window���J���{�^��")]
    public List<GameObject> windowOpener = new List<GameObject>();
    [Header("Window�����{�^��")]
    public List<GameObject> windowCloser = new List<GameObject>();
    [Header("Window�����w�i�{�^��")]
    public GameObject backWindowCloser;
    [Header("Window�̔w�i")]
    public GameObject backgroundPanel;
    [Header("Window")]
    public GameObject windowPanel;
    [Header("Window�̍��v�f�S��")]
    public GameObject panel;

    private RectTransform windowRect;
    private Vector3 defaultScale;
    private Image backImage;
    private bool isAnimate = false;

    private void Awake()
    {
        panel.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        windowRect = windowPanel.GetComponent<RectTransform>();
        defaultScale = windowRect.localScale;
        windowRect.localScale = Vector3.zero;

        backImage = backgroundPanel.GetComponent<Image>();
        backImage.color = Color.clear;

        // Window���J�����߂̃{�^��
        if (windowOpener != null)
        {
            foreach(GameObject g in windowOpener) g.GetComponent<Button>().onClick.AddListener(OpenPanel);
        }

        // Window����邽�߂̃{�^��
        if (windowCloser != null)
        {
            foreach (GameObject g in windowCloser) g.GetComponent<Button>().onClick.AddListener(ClosePanel);
        }

        // Window����邽�߂̔w�i�{�^��
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
    /// Window���J��
    /// </summary>
    public void OpenPanel()
    {
        backWindowCloser.SetActive(true);
        isAnimate = true;
        windowRect.DOScale(defaultScale, animationTime).SetEase(Ease.OutBack);
        backImage.DOFade(0.8f, animationTime).OnComplete(() => isAnimate = false);
    }

    /// <summary>
    /// Window�����
    /// </summary>
    public void ClosePanel()
    {
        backWindowCloser.SetActive(false);
        isAnimate = true;
        windowRect.DOScale(0, animationTime).SetEase(Ease.InBack);
        backImage.DOFade(0, animationTime).OnComplete(() => isAnimate = false);
    }

    /// <summary>
    /// Window������Ȃ����Ƃ𑣂�
    /// </summary>
    public void DontClosePanel()
    {
        windowRect.DOPunchScale(Vector3.one * 0.05f, animationTime).OnComplete(() => windowRect.DOScale(Vector3.one, 0.001f));
    }
}
