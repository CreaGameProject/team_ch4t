using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectView : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> characterNode = new List<GameObject>();
    [SerializeField]
    private GameObject characterNodeParent;
    ScrollRect scrollRect;

    [SerializeField]
    float fadeinSeparateDuration = 0.1f;
    [SerializeField]
    float fadeinDuration = 0.2f;
    [SerializeField]
    GameObject characterContent;

    private List<GameObject> releasedCharacterNode = new List<GameObject>();
    private int characterCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.horizontalNormalizedPosition = 0;

        /*
        foreach (GameObject characterNode in releasedCharacterNode)
        {
            characterNode.GetComponent<CanvasGroup>().alpha = 0;
            characterNode.transform.GetChild(0).transform.localScale = Vector3.zero;
        }

        var characterSelectAnim = DOTween.Sequence();

        foreach (GameObject characterNode in releasedCharacterNode)
        {
            characterSelectAnim.Append(characterNode.GetComponent<CanvasGroup>().DOFade(1, fadeinDuration));
            characterSelectAnim.Join(characterNode.transform.GetChild(0).transform.DOScale(1, fadeinDuration).SetEase(Ease.OutBack));
        }
        */
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddOpponent(Computer.Opponent opponent)
    {
        GameObject node = Instantiate(characterNode[((int)opponent)], characterNodeParent.transform);
        releasedCharacterNode.Add(node);

        node.GetComponent<CanvasGroup>().alpha = 0;
        node.transform.GetChild(0).transform.localScale = Vector3.zero;

        var characterSelectAnim = DOTween.Sequence();
        characterSelectAnim.Append(node.GetComponent<CanvasGroup>().DOFade(1, fadeinDuration));
        characterSelectAnim.Join(node.transform.GetChild(0).transform.DOScale(1, fadeinDuration).SetEase(Ease.OutBack));

        characterCount++;
    }
}
