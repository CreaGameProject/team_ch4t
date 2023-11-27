using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class DialoguePresenter : MonoBehaviour
{
    [SerializeField] private DialogueModel _model;
    [SerializeField] private DialogueView _view;
    [SerializeField] private Board _board;

    private BackLogData _backLogData;
    public BackLogData BackLogData => _backLogData;
    
    
    private async void Start()
    {
        
        _backLogData = new BackLogData();

        _board.OnSpeakComputerExecuted += SpeakComputerEventHandler;
        _board.OnSecretCellPerformanceExecuted += SecretCellPerformanceEventHandler;
        
        _view.PrefixBattleDialogue(
            _model.DialogueTalkEvents[0].CharacterName,
            "Sprites/CAS_character_portraits_for_dialogs_vol1/" +  _model.DialogueTalkEvents[0].FilePath
            );
    }
    
    private async UniTask SpeakComputerEventHandler()
    {
        await StartRandomBattleDialogue(Board.instance.getHowManyHimituDidGet);
    }

    private async UniTask SecretCellPerformanceEventHandler()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        await StartCutIn(Board.instance.getHowManyHimituDidGet, token);
        
        cts.Cancel();
    }

    private async UniTask StartRandomBattleDialogue(int secretCount)
    {
        var selectedTalkEvents = _model.DialogueTalkEvents
            .Where(dialogueTalkEvent => dialogueTalkEvent.SecretCount == secretCount)
            .ToList();
        
        var randomOrder = new Random();
        var randomlyOrderedTalkEvents = selectedTalkEvents.OrderBy(x => randomOrder.Next()).ToList();

        // ランダムに1つの要素を抽出
        var randomlySelectedTalkEvent = randomlyOrderedTalkEvents.FirstOrDefault();
        
        if (randomlySelectedTalkEvent != null)
        {
            await StartBattleDialogue(randomlySelectedTalkEvent);
        }
    }

    private async UniTask StartBattleDialogue(DialogueTalkEvent talkEvent)
    {
        // TODO: 本番環境に差し替え
        await _view.StartBattleDialogue(
            talkEvent.CharacterName,
            "Sprites/CAS_character_portraits_for_dialogs_vol1/" + talkEvent.FilePath,
            talkEvent.Text
        );
    }

    private async UniTask StartCutIn(int secretCount, CancellationToken token)
    {
        var selectedCutInEvents = _model.DialogueCutInEvents
            .Where(dialogueTalkEvent => dialogueTalkEvent.SecretCount == secretCount)
            .ToList();

        var cutInEvent = selectedCutInEvents[0];
        await _view.StartCutIn(
            "Sprites/CAS_character_portraits_for_dialogs_vol1/" + cutInEvent.FilePath,
            token
        );

    }
}
