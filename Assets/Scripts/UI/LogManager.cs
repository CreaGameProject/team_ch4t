using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogManager : MonoBehaviour
{
    [SerializeField]
    private GameObject nodeGroup;
    [SerializeField]
    private GameObject narratorLogNode;
    [SerializeField]
    private GameObject uiLogNode, bossLogNode, puppuLogNode;
    [SerializeField]
    private GameObject scrollView;
    private ScrollRect scrollRect;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = scrollView.GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// ��b���O�ɔ�����ǉ�
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="log">���Ȃ��̔���</param>
    public void AddLog(string name, string log)
    {
        GameObject sb = null;
        switch (name)
        {
            case "ui": sb = Instantiate(uiLogNode, nodeGroup.transform); break;
            case "boss": sb = Instantiate(bossLogNode, nodeGroup.transform); break;
            case "puppu": sb = Instantiate(puppuLogNode, nodeGroup.transform); break;
            case "narrator": sb = Instantiate(narratorLogNode, nodeGroup.transform); break;
        }
        sb.GetComponent<LogNode>().SetContent(log);
        StartCoroutine(AnimateScrollView());*/
    }

    private IEnumerator AnimateScrollView()
    {
        yield return new WaitForSeconds(0.1f);
        nodeGroup.GetComponent<VerticalLayoutGroup>().spacing -= 0.0001f;
        scrollRect.DOVerticalNormalizedPos(0, 2);
    }
}
