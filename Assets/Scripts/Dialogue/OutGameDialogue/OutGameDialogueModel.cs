using System;
using System.Collections.Generic;
using UnityEngine;

public class OutGameDialogueModel : MonoBehaviour
{
    private List<AbstractDialogueEvent> dialogueEvents = new List<AbstractDialogueEvent>();
    private List<List<AbstractDialogueEvent>> dialogueEventsList = new List<List<AbstractDialogueEvent>>(); 
    private BackLogData _backLogData;
    public BackLogData BackLogData => _backLogData;
    
    private void Awake()
    {
        PrefixDialogueEventList();
    }

    private void PrefixDialogueEventList()
    {
        var jsonFile = Resources.Load<TextAsset>(Helper.OutGameDialogueJsonPath);
        
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            var dialogueData = JsonUtility.FromJson<OutGameDialogueJsonHolder>(jsonText);

            foreach (var dialogueJson in dialogueData.dialogueEvents)
            {
                DialogueEventType dialogueEventType = Enum.Parse<DialogueEventType>(dialogueJson.type);

                switch (dialogueEventType)
                {
                    case DialogueEventType.TALK:
                        var dialogueTalk = new OutGameDialogueTalkEvent(
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
                        dialogueEventsList[dialogueTalk.EventID].Add(dialogueTalk);
                        //DialogueTalkEvents.Add(dialogueTalk);
                        break;
                    case DialogueEventType.CUT_IN:
                        //var dialogueCutIn = new DialogueCutInEvent(dialogueJson.event_number, dialogueEventType, dialogueJson.secret_count, dialogueJson.name, dialogueJson.file);
                        //DialogueCutInEvents.Add(dialogueCutIn);
                        break;
                    case DialogueEventType.CUT_IN_TALK:
                        //var dialogueCutInTalk = new DialogueCutInTalkEvent(dialogueJson.event_number, dialogueEventType, dialogueJson.secret_count, dialogueJson.name, dialogueJson.file, dialogueJson.text);
                        //DialogueCutInTalkEvents.Add(dialogueCutInTalk);
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
    
    public void AddBackLogData(string characterName, string dialogue)
    {
        var logData = new BackLogData.LogData
        {
            speaker = characterName,
            dialogue = dialogue
        };

        _backLogData.logDataList.Add(logData);
        //Debug.Log($"バックログにspeaker: {logData.speaker}, dialogue: {logData.dialogue} を追加しました。");
    }
}
