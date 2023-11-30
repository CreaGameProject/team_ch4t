using System;
using UnityEngine;

public class OutGameDialoguePresenter : MonoBehaviour
{
    [SerializeField] private OutGameDialogueModel _model;
    [SerializeField] private OutGameDialogueView _view;

    private async void Start()
    {
        _view.OnCharacterTalkExecuted += CharacterTalkExecutedEventHandler;

        foreach (var dialogueEvents in _model.dialogueEventsList)
        {
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
                    case DialogueEventType.BLACKOUT:
                        OutGameDialogueBlackoutEvent blackoutEvent = (OutGameDialogueBlackoutEvent)dialogueEvent;
                        await _view.PlayBlackout(blackoutEvent.FilePath);
                        break;
                }
            }
        }
    }
    
    private void CharacterTalkExecutedEventHandler(string characterName, string dialogue)
    {
        _model.AddBackLogData(characterName, dialogue);
    }
}
