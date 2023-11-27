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

    private BackLogData presenter;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = scrollView.GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadLog()
    {
        foreach (Transform n in nodeGroup.transform)
        {
            Destroy(n.gameObject);
        }

        for (int i = 0; i < presenter.logDataList.Count; i++)
        {
            AddLog(presenter.logDataList[i].speaker, presenter.logDataList[i].dialogue);
        }
    }

    /// <summary>
    /// âÔòbÉçÉOÇ…î≠åæÇí«â¡
    /// </summary>
    /// <param name="name">î≠åæé“</param>
    /// <param name="log">î≠åæ</param>
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
        StartCoroutine(AnimateScrollView());
    }

    private IEnumerator AnimateScrollView()
    {
        yield return new WaitForSeconds(0.1f);
        nodeGroup.GetComponent<VerticalLayoutGroup>().spacing -= 0.0001f;
        scrollRect.DOVerticalNormalizedPos(0, 2);
    }
}
