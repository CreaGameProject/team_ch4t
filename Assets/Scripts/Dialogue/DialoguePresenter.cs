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
    [SerializeField] private DialogueJsonReader _dialogueJsonReader;
    
    private async void Start()
    {
        //await _view.StartBattleDialogue("テストキャラクラー","Sprites/CAS_character_portraits_for_dialogs_vol1portrait_kohaku_02","テキストてきすと<color=#9c3444>文章</color>text");
        //await _view.StartBattleDialogue("テストキャラクター","Sprites/CAS_character_portraits_for_dialogs_vol1/portrait_kohaku_03","テキストてきすと<color=#9c3444>文章</color>text");
        //Debug.Log("話終わりました。");
        await StartRandomBattleDialogue(_dialogueJsonReader.DialogueTalkEvents, 0);
    }

    public async UniTask StartRandomBattleDialogue(List<DialogueTalkEvent> dialogueTalkEvents ,int secretCount)
    {
        var selectedTalkEvents = dialogueTalkEvents
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

    public async UniTask StartBattleDialogue(DialogueTalkEvent talkEvent)
    {
        // TODO: 本番環境に差し替え
        await _view.StartBattleDialogue(
            talkEvent.CharacterName,
            "Sprites/CAS_character_portraits_for_dialogs_vol1/" + talkEvent.FilePath,
            talkEvent.Text
        );
    }
}
