using System;
using System.Threading;
using UnityEngine;

public class DialogueScenePresenter: MonoBehaviour
{
    [SerializeField] private OutGameDialoguePresenter _presenter;
    [SerializeField] private SceneChanger _sceneChanger;

    private CancellationTokenSource cts;

    
    private async void Start()
    {
        AudioManager.instance_AudioManager.PlayBGM(1);
        
        cts = new CancellationTokenSource();
        var token = cts.Token;

        await _presenter.PlayDialogue(0, token);
        cts.Cancel();
        
        _sceneChanger.LoadScene("ReversiBasic");
    }
    
    void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
