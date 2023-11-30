using System;
using System.Collections.Generic;
using UnityEngine;

public class OutGameDialogueModel : DialogueModelBase
{
    public List<List<AbstractDialogueEvent>> dialogueEventsList = new List<List<AbstractDialogueEvent>>();
    
    private void Awake()
    {
        PrefixDialogueEventList();
    }

    private void PrefixDialogueEventList()
    {
        var maxEventID = 0;
        var jsonFile = Resources.Load<TextAsset>(Helper.OutGameDialogueJsonPath);
        
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            var dialogueData = JsonUtility.FromJson<OutGameDialogueJsonHolder>(jsonText);

            foreach (var dialogueJson in dialogueData.dialogueEvents)
            {
                DialogueEventType dialogueEventType = Enum.Parse<DialogueEventType>(dialogueJson.type);

                if (maxEventID <= dialogueJson.event_id)
                {
                    maxEventID = dialogueJson.event_id;
                    dialogueEventsList.Add(new List<AbstractDialogueEvent>());
                }

                AbstractDialogueEvent dialogueEvent;
                switch (dialogueEventType)
                {
                    case DialogueEventType.TALK:
                        dialogueEvent = new OutGameDialogueTalkEvent(
                            dialogueJson.event_number,
                            dialogueEventType,
                            dialogueJson.event_id,
                            dialogueJson.name,
                            dialogueJson.file,
                            dialogueJson.name_sub,
                            dialogueJson.file_sub,
                            dialogueJson.talker,
                            dialogueJson.text
                        );
                        dialogueEventsList[dialogueJson.event_id].Add(dialogueEvent);
                        break;
                    case DialogueEventType.ITEM:
                        dialogueEvent = new OutGameDialogueItemEvent(
                            dialogueJson.event_number,
                            dialogueEventType,
                            dialogueJson.event_id,
                            dialogueJson.file);
                        dialogueEventsList[dialogueJson.event_id].Add(dialogueEvent);
                        break;
                    case DialogueEventType.SOUND:
                        dialogueEvent = new OutGameDialogueSoundEvent(
                            dialogueJson.event_number,
                            dialogueEventType,
                            dialogueJson.event_id,
                            dialogueJson.file,
                            dialogueJson.text);
                        dialogueEventsList[dialogueJson.event_id].Add(dialogueEvent);
                        break;
                    case DialogueEventType.BLACKOUT:
                        dialogueEvent = new OutGameDialogueBlackoutEvent(
                            dialogueJson.event_number,
                            dialogueEventType,
                            dialogueJson.event_id,
                            dialogueJson.file);
                        dialogueEventsList[dialogueJson.event_id].Add(dialogueEvent);
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            Debug.LogError("JSONファイルが割り当てられていません。");
        }
    }
}
