using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class DialogueJsonReader : MonoBehaviour
{
    private DialogueJsonHolder dialogueData;
    public List<DialogueTalkEvent> DialogueTalkEvents = new List<DialogueTalkEvent>();
    private List<AbstractDialogueEvent> dialogueCutInEvents = new List<AbstractDialogueEvent>();
    void Start()
    {
        var jsonFile = Resources.Load<TextAsset>("JSON/text_event_test");
        
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            dialogueData = JsonUtility.FromJson<DialogueJsonHolder>(jsonText);

            foreach (var dialogueJson in dialogueData.dialogueEvents)
            {
                EventType eventType = Enum.Parse<EventType>(dialogueJson.type);
                
                AbstractDialogueEvent dialogueEvent;
                switch (eventType)
                {
                    case EventType.TALK:
                        DialogueTalkEvent dialogueTalkEvent = new DialogueTalkEvent(dialogueJson.event_number, eventType, dialogueJson.secret_count ,dialogueJson.name, dialogueJson.file, dialogueJson.text);
                        DialogueTalkEvents.Add(dialogueTalkEvent);
                        break;
                    case EventType.CUT_IN:
                        dialogueEvent = new DialogueCutInEvent(dialogueJson.event_number, eventType, dialogueJson.name, dialogueJson.file);
                        dialogueCutInEvents.Add(dialogueEvent);
                        break;
                    case EventType.CUT_IN_TALK:
                        dialogueEvent = new DialogueCutInTalkEvent(dialogueJson.event_number, eventType, dialogueJson.name, dialogueJson.file, dialogueJson.text);
                        dialogueCutInEvents.Add(dialogueEvent);
                        break;
                    case EventType.END:
                        dialogueEvent = new DialogueEndEvent(dialogueJson.event_number, eventType);
                        dialogueCutInEvents.Add(dialogueEvent);
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
