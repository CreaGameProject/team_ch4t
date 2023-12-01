using System.Threading;
using UnityEngine;

public class ResultScenePresenter : MonoBehaviour
{
    [SerializeField] private OutGameDialoguePresenter _presenter;
    [SerializeField] private ResultSceneView _view;
    [SerializeField] private SceneChanger _sceneChanger;
    
    
    private CancellationTokenSource cts;
    private bool isReported = false;
    
    private async void Start()
    {
        _view.OnClicked += OnReportButtonClicked;
        
        AudioManager.instance_AudioManager.PlayBGM(1);
        
        cts = new CancellationTokenSource();
        var token = cts.Token;

        await _presenter.PlayDialogue(1, token);
        await _view.ShowResult();

        if (isReported)
        {
            await _presenter.PlayDialogue(2, token);
        }
        else
        {
            await _presenter.PlayDialogue(3, token);
        }

        await _presenter.PlayDialogue(4, token);
        
        cts.Cancel();
        _sceneChanger.LoadScene("Title");
    }
    
    void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    void OnReportButtonClicked(bool isReport)
    {
        isReported = isReport;
    }
}