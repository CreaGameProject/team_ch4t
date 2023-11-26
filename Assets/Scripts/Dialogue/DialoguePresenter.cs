using System;
using System.Collections.Generic;
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
        
    }

    private async UniTask StartRandomBattleDialogue(List<DialogueTalkEvent> dialogueTalkEvents ,int secretCount)
    {
        Random random = new Random();
        int r =  random.Next(dialogueTalkEvents.Count);

        DialogueTalkEvent talkEvent = _dialogueJsonReader.DialogueTalkEvents[r];
        if (talkEvent.SecretCount == 0)
        {
            await StartBattleDialogue(talkEvent);
        }
    }

    public async UniTask StartBattleDialogue(DialogueTalkEvent talkEvent)
    {
        await _view.StartBattleDialogue(
            talkEvent.CharacterName,
            "Sprites/CAS_character_portraits_for_dialogs_vol1/" + talkEvent.FilePath,
            talkEvent.Text
        );
    }
}
