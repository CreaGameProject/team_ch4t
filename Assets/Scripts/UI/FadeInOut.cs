using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [SerializeField]
    private float fadeDuration = 1;
    [SerializeField]
    private GameObject resultPanel;

    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 画面を暗転させてリザルト画面へ遷移
    /// </summary>
    public void GoDarkForTransition()
    {
        Sequence fadeAnim = DOTween.Sequence();
        fadeAnim.Append(image.DOFade(1, fadeDuration))
                .AppendCallback(() => resultPanel.SetActive(false))
                .Append(image.DOFade(0, fadeDuration));
    }
}
