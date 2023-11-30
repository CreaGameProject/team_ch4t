using UnityEngine;
using DG.Tweening;

public class SecretWindow: MonoBehaviour
{
    public float animationTime = 0.3f;
    [SerializeField]
    private float windowScaleRatio = 2;
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
}
