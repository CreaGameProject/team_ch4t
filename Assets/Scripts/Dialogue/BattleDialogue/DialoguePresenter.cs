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
    
    private async void Start()
    {
        _view.OnCharacterTalkExecuted += CharacterTalkExecutedEventHandler;
        _board.OnSpeakComputerExecuted += SpeakComputerEventHandler;
        _board.OnSecretCellPerformanceExecuted += SecretCellPerformanceEventHandler;
        
        _view.PrefixBattleDialogue(
            _model.DialogueTalkEvents[0].CharacterName,
            _model.DialogueTalkEvents[0].FilePath
            );
        
        AudioManager.instance_AudioManager.PlayBGM(0);
    }

    private void CharacterTalkExecutedEventHandler(string characterName, string dialogue)
    {
        _model.AddBackLogData(characterName, dialogue);
    }
    
    private async UniTask SpeakComputerEventHandler()
    {
        await StartRandomBattleDialogue();
    }

    private async UniTask SecretCellPerformanceEventHandler()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        var secretCount = Board.instance.getHowManyHimituDidGet;

        await UniTask.WhenAll(
             StartCutIn(secretCount, token),
             PrefixCutInTalk(secretCount)
            );

        await StartCutInTalk(secretCount);
        
        cts.Cancel();
    }

    private async UniTask StartRandomBattleDialogue()
    {
        
        // ランダムに1つの要素を抽出
        var randomlySelectedTalkEvent = _model.randomlyOrderedTalkEvents.FirstOrDefault();
        if (randomlySelectedTalkEvent != null)
        {
            _model.randomlyOrderedTalkEvents.Remove(randomlySelectedTalkEvent);
            await StartBattleDialogue(randomlySelectedTalkEvent);
        }
        else
        {
            _model.SetRandomlyOrderedTalkEvents();
            await StartRandomBattleDialogue();
        }
    }

    private async UniTask StartBattleDialogue(DialogueTalkEvent talkEvent)
    {
        await _view.StartBattleDialogue(
            talkEvent.CharacterName,
            talkEvent.FilePath,
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
            cutInEvent.FilePath,
            token
        );
    }

    private async UniTask PrefixCutInTalk(int secretCount)
    {
        var selectedCutInTalkEvents = _model.DialogueCutInTalkEvents
            .Where(dialogueTalkEvent => dialogueTalkEvent.SecretCount == secretCount)
            .ToList();

        var cutInTalkEvent = selectedCutInTalkEvents[0];

        await _view.PrefixCutInTalkDialogue(
            cutInTalkEvent.CharacterName,
            cutInTalkEvent.FilePath
        );
    }

    private async UniTask StartCutInTalk(int secretCount)
    {
        var selectedCutInTalkEvents = _model.DialogueCutInTalkEvents
            .Where(dialogueTalkEvent => dialogueTalkEvent.SecretCount == secretCount)
            .ToList();
        
        foreach (var cutInTalkEvent in selectedCutInTalkEvents)
        {
            await _view.StartCutInTalkDialogue(
                cutInTalkEvent.CharacterName,
                cutInTalkEvent.FilePath,
                cutInTalkEvent.Text
            );
        }

        _view.CloseCutInTalkDialogue();
    }
}
