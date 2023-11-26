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
    private GameObject yourLogNode;
    [SerializeField]
    private List<GameObject> partnerLogNode;
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            AddPartnerLog("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            AddYourLog("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
        }
    }

    /// <summary>
    /// ��b���O�ɂ��Ȃ��̔�����ǉ�
    /// </summary>
    /// <param name="log">���Ȃ��̔���</param>
    public void AddYourLog(string log)
    {
        GameObject sb =  Instantiate(yourLogNode, nodeGroup.transform);
        sb.GetComponent<LogNode>().SetContent(log);
        StartCoroutine(AnimateScrollView());
    }

    /// <summary>
    /// ��b���O�ɑ���̔�����ǉ�
    /// </summary>
    /// <param name="log">����̔���</param>
    public void AddPartnerLog(string log)
    {
        /*GameObject sb =  Instantiate(partnerLogNode, nodeGroup.transform);
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
