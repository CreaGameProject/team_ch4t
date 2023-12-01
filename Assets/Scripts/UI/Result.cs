using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField]
    private GameObject fadeInOut;
    [SerializeField]
    private float fadeDuration = 2;
    [SerializeField]
    private GameObject[] kaltes = new GameObject[2];
    [SerializeField]
    private GameObject backGround;
    private Image fadeInOutImage;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
        fadeInOutImage = fadeInOut.GetComponent<Image>();
        backGround.SetActive(false);

        /*
        Computer.Opponent opponent = Board.instance.getOpponent;
        switch (opponent)
        {
            case Computer.Opponent.Yukihira_ui:
                kaltes[0].SetActive(true);
                break;
            case Computer.Opponent.Takahashi_shota:
                kaltes[1].SetActive(true);
                break;
        }
        */

        kaltes[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// リザルトを表示
    /// </summary>
    public void GoToResultView()
    {
        /*
        Sequence fadeAnim = DOTween.Sequence();
        fadeAnim.Append(fadeInOutImage.DOFade(1, fadeDuration))
                .AppendCallback(() => transform.localScale = Vector3.one)
                .Append(fadeInOutImage.DOFade(0, fadeDuration));
                */
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// リザルト画面の次へ
    /// </summary>
    public void GoToTalkView()
    {
        /*
        Sequence fadeAnim = DOTween.Sequence();
        backGround.SetActive(true);
        fadeAnim.Append(fadeInOutImage.DOFade(1, fadeDuration))
                .AppendCallback(() => transform.localScale = Vector3.zero)
                .Append(fadeInOutImage.DOFade(0, fadeDuration));
                }*/
        transform.localScale = Vector3.zero;
    }
}
