using UnityEngine;

public class DialogueJsonReader : MonoBehaviour
{
    void Start()
    {
        var jsonFile = Resources.Load<TextAsset>("JSON/text_event_test");
        
        if (jsonFile != null)
        {
            string jsonText = jsonFile.text;
            DialogueEventHolder dialogueData = JsonUtility.FromJson<DialogueEventHolder>(jsonText);

            foreach (var dialogueEvent in dialogueData.dialogueEvents)
            {
                Debug.Log("Event Number: " + dialogueEvent.eventNumber);
                Debug.Log("Type: " + dialogueEvent.eventType);
                Debug.Log("Name: " + dialogueEvent.name);
                Debug.Log("File: " + dialogueEvent.filePath);
                Debug.Log("Text: " + dialogueEvent.text);
            }
        }
        else
        {
            Debug.LogError("JSONファイルが割り当てられていません。");
        }
    }
}
