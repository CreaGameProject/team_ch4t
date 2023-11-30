using System.Collections;
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

    [SerializeField] private DialogueModelBase _model;

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

        for (int i = 0; i < _model.BackLogData.logDataList.Count; i++)
        {
            AddLog(_model.BackLogData.logDataList[i].speaker, _model.BackLogData.logDataList[i].dialogue);
        }
    }

    /// <summary>
    /// 会話ログに発言を追加
    /// </summary>
    /// <param name="name">発言者</param>
    /// <param name="log">発言</param>
    public void AddLog(string name, string log)
    {
        GameObject sb = null;
        switch (name)
        {
            case "うい": sb = Instantiate(uiLogNode, nodeGroup.transform); break;
            case "部長": sb = Instantiate(bossLogNode, nodeGroup.transform); break;
            case "プップ": case "？？？": sb = Instantiate(puppuLogNode, nodeGroup.transform); break;
            default: sb = Instantiate(narratorLogNode, nodeGroup.transform); break;
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
