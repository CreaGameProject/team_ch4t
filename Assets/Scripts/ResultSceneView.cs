using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ResultSceneView : MonoBehaviour
{
    [SerializeField] private Result _resultView;
    [SerializeField] private CustomButton reportButton;
    [SerializeField] private CustomButton nonReportButton;

    private async void Start()
    {
        reportButton.OnClickAction = () => ReportButtonClicked(true);
        nonReportButton.OnClickAction = () => ReportButtonClicked(false);
    }

    public async UniTask ShowResult()
    {
        _resultView.GoToResultView();
        await UniTask.WhenAny(
            reportButton.OnClickAsync(),
            nonReportButton.OnClickAsync()
            );
        
        _resultView.GoToTalkView();
    }
    
    /// <summary>
    /// キャラクターが話終えたときに呼び出される
    /// </summary>
    public delegate void OnClickedDelegate(bool isReport);

    public event OnClickedDelegate OnClicked;

    private void ReportButtonClicked(bool isReport)
    {
        
        if (OnClicked != null) { OnClicked(isReport); }
    }
}
