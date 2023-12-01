using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ResultScenePresenter : MonoBehaviour
{
    [SerializeField] private OutGameDialoguePresenter _presenter;
    [SerializeField] private ResultSceneView _view;
    [SerializeField] private SceneChanger _sceneChanger;
    
    
    private CancellationTokenSource cts;
    private bool isReported;
    
    private async void Start()
    {
        _view.OnClicked += OnReportButtonClicked;
        
        AudioManager.instance_AudioManager.PlayBGM(1);
        
        cts = new CancellationTokenSource();
        var token = cts.Token;

        await _presenter.PlayDialogue(1, token);
        
        await UniTask.Delay(10);
        await _view.ShowResult();
        await UniTask.Delay(10);
        
        if (isReported)
        {
            Debug.Log($"await _presenter.PlayDialogue(2, token); :{isReported}");
            await _presenter.PlayDialogue(2, token);
        }
        else
        {
            Debug.Log($"await _presenter.PlayDialogue(3, token); :{isReported}");
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
        Debug.Log($"OnReportButtonClicked:{isReported}");
    }
}