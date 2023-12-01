using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class OutGameDialoguePresenter : MonoBehaviour
{
    [SerializeField] private OutGameDialogueModel _model;
    [SerializeField] private OutGameDialogueView _view;

    private async void Start()
    {
        _view.OnCharacterTalkExecuted += CharacterTalkExecutedEventHandler;
        _view.OnDialogueEnd += DialogueEndEventHandler;
    }

    public async UniTask PlayDialogue(int eventID, CancellationToken token)
    {
        var dialogueEvents = _model.dialogueEventsList[eventID];
        foreach (var dialogueEvent in dialogueEvents)
        {
            switch (dialogueEvent.DialogueEventType)
            {
                case DialogueEventType.TALK:
                    OutGameDialogueTalkEvent talkEvent = (OutGameDialogueTalkEvent)dialogueEvent;
                    await _view.StartDialogue(talkEvent.CharacterName, talkEvent.FilePath,
                        talkEvent.CharacterNameSub, talkEvent.FilePathSub, talkEvent.TalkerNumber, talkEvent.Text);
                    break;
                case DialogueEventType.ITEM:
                    OutGameDialogueItemEvent itemEvent = (OutGameDialogueItemEvent)dialogueEvent;
                    await _view.ShowItem(itemEvent.FilePath);
                    break;
                case DialogueEventType.SOUND:
                    OutGameDialogueSoundEvent soundEvent = (OutGameDialogueSoundEvent)dialogueEvent;
                    await _view.PlaySound(soundEvent.FilePath, soundEvent.Text);
                    break;
                case DialogueEventType.FADE_IN:
                    OutGameDialogueFadeInEvent fadeInEvent = (OutGameDialogueFadeInEvent)dialogueEvent;
                    await _view.PlayFadeIn(fadeInEvent.FilePath, token);
                    break;
                case DialogueEventType.FADE_OUT:
                    await _view.PlayFadeOut(token);
                    break;
                case DialogueEventType.END:
                    _view.OnEnd();
                    break;
            }
        }
    }
    
    private void CharacterTalkExecutedEventHandler(string characterName, string dialogue)
    {
        _model.AddBackLogData(characterName, dialogue);
    }

    private void DialogueEndEventHandler()
    {
        // TODO: ENDイベントの内容の変更
        /*var cts = new CancellationTokenSource();
        var token = cts.Token;
        PlayDialogue(_model.dialogueEventsList[1], token);
        */
    }
}
