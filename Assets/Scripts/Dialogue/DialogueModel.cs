using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class DialogueModel : MonoBehaviour
{
    private DialogueJsonHolder dialogueData;
    public List<DialogueTalkEvent> DialogueTalkEvents = new List<DialogueTalkEvent>();
    public List<DialogueCutInEvent> DialogueCutInEvents = new List<DialogueCutInEvent>();
    public List<DialogueCutInTalkEvent> DialogueCutInTalkEvents = new List<DialogueCutInTalkEvent>();
    
    private BackLogData _backLogData;
    public BackLogData BackLogData => _backLogData;
    
    void Awake()
    {
        PrefixDialogueEventList();
    }

    private void Start()
    {
        _backLogData = new BackLogData();
    }

    private void PrefixDialogueEventList()
    {
        var jsonFile = Resources.Load<TextAsset>(Helper.BattleDialogueJsonPath);
        
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            dialogueData = JsonUtility.FromJson<DialogueJsonHolder>(jsonText);

            foreach (var dialogueJson in dialogueData.dialogueEvents)
            {
                EventType eventType = Enum.Parse<EventType>(dialogueJson.type);

                switch (eventType)
                {
                    case EventType.TALK:
                        var dialogueTalk = new DialogueTalkEvent(dialogueJson.event_number, eventType, dialogueJson.secret_count, dialogueJson.name, dialogueJson.file, dialogueJson.text);
                        DialogueTalkEvents.Add(dialogueTalk);
                        break;
                    case EventType.CUT_IN:
                        var dialogueCutIn = new DialogueCutInEvent(dialogueJson.event_number, eventType, dialogueJson.secret_count, dialogueJson.name, dialogueJson.file);
                        DialogueCutInEvents.Add(dialogueCutIn);
                        break;
                    case EventType.CUT_IN_TALK:
                        var dialogueCutInTalk = new DialogueCutInTalkEvent(dialogueJson.event_number, eventType, dialogueJson.secret_count, dialogueJson.name, dialogueJson.file, dialogueJson.text);
                        DialogueCutInTalkEvents.Add(dialogueCutInTalk);
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
        Debug.Log($"バックログにspeaker: {logData.speaker}, dialogue: {logData.dialogue} を追加しました。");
    }
}
