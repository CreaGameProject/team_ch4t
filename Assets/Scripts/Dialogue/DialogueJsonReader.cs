using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueJsonReader : MonoBehaviour
{
    private DialogueJsonHolder dialogueData;
    [SerializeField] private List<AbstractDialogueEvent> dialogueEvents = new List<AbstractDialogueEvent>();
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

                AbstractDialogueEvent dialogueEvent = null;
                switch (eventType)
                {
                    case EventType.TALK:
                        dialogueEvent = new DialogueTalkEvent(dialogueJson.event_number, eventType, dialogueJson.name, dialogueJson.file, dialogueJson.text);
                        dialogueEvents.Add(dialogueEvent);
                        break;
                    case EventType.CUT_IN:
                        dialogueEvent = new DialogueCutInEvent(dialogueJson.event_number, eventType, dialogueJson.name, dialogueJson.file, dialogueJson.text);
                        dialogueEvents.Add(dialogueEvent);
                        break;
                    case EventType.END:
                        dialogueEvent = new DialogueEndEvent(dialogueJson.event_number, eventType);
                        dialogueEvents.Add(dialogueEvent);
                        break;
                    default:
                        break;
                }
            }

            foreach (var dialogueEvent in dialogueEvents)
            {
                Debug.Log(dialogueEvent.ToString());
            }
        }
        else
        {
            Debug.LogError("JSONファイルが割り当てられていません。");
        }
    }
}
