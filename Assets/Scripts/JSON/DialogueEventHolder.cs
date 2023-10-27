using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueEventHolder : MonoBehaviour
{
    private TextAsset jsonAsset;
    [SerializeField] List<DialogueEvent> dialogueEventList;

    private void Start()
    {
        jsonAsset = Resources.Load<TextAsset>("JSON/text_event_test");
        
        if (jsonAsset != null)
        {
            // JSONデータをデシリアライズしてリストに格納
            dialogueEventList = JsonUtility.FromJson<List<DialogueEvent>>(jsonAsset.text);
            foreach (var dialogueEvent in dialogueEventList)
            {
                Debug.Log(dialogueEvent.eventNumber);
            }
        }
        else
        {
            Debug.LogError("取得できませんでした");
        }
    }
}
