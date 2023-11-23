using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [SerializeField]
    private float fadeDuration = 1;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeOut()
    {
        image.DOFade(1, fadeDuration);
    }

    public void FadeIn()
    {
        image.DOFade(0, fadeDuration);
    }
}
