using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFadeIner : MonoBehaviour
{
    [SerializeField]
    float fadeinSeparateDuration = 0.1f;
    [SerializeField]
    float fadeinDuration = 0.2f;
    [SerializeField]
    GameObject characterContent;

    List<GameObject> characterNode = new List<GameObject>();
    int characterCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        characterCount = characterContent.transform.childCount;
        for (int i = 0; i < characterCount; i++)
        {
            characterNode.Add(characterContent.transform.GetChild(i).gameObject);
        }

        foreach (GameObject characterNode in characterNode)
        {
            characterNode.GetComponent<CanvasGroup>().alpha = 0;
            characterNode.transform.GetChild(0).transform.localScale = Vector3.zero;
        }

        var characterSelectAnim = DOTween.Sequence();

        foreach(GameObject characterNode in characterNode)
        {
            characterSelectAnim.Append(characterNode.GetComponent<CanvasGroup>().DOFade(1, fadeinDuration));
            characterSelectAnim.Join(characterNode.transform.GetChild(0).transform.DOScale(1, fadeinDuration).SetEase(Ease.OutBack));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
