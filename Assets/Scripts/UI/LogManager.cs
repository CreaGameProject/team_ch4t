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

    [SerializeField] private DialogueModel _model;

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
    /// ��b���O�ɔ�����ǉ�
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="log">����</param>
    public void AddLog(string name, string log)
    {
        GameObject sb = null;
        switch (name)
        {
            case "����": sb = Instantiate(uiLogNode, nodeGroup.transform); break;
            case "����": sb = Instantiate(bossLogNode, nodeGroup.transform); break;
            case "�v�b�v": sb = Instantiate(puppuLogNode, nodeGroup.transform); break;
            case "": sb = Instantiate(narratorLogNode, nodeGroup.transform); break;
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
