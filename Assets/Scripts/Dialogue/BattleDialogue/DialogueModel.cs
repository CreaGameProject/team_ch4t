using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DialogueModel : DialogueModelBase
{
    private DialogueJsonHolder dialogueData;
    public List<DialogueTalkEvent> DialogueTalkEvents = new List<DialogueTalkEvent>();
    public List<DialogueCutInEvent> DialogueCutInEvents = new List<DialogueCutInEvent>();
    public List<DialogueCutInTalkEvent> DialogueCutInTalkEvents = new List<DialogueCutInTalkEvent>();
    public List<DialogueTalkEvent> randomlyOrderedTalkEvents = new List<DialogueTalkEvent>();
    
    void Awake()
    {
        PrefixDialogueEventList();
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
                DialogueEventType dialogueEventType = Enum.Parse<DialogueEventType>(dialogueJson.type);

                switch (dialogueEventType)
                {
                    case DialogueEventType.TALK:
                        var dialogueTalk = new DialogueTalkEvent(dialogueJson.event_number, dialogueEventType, dialogueJson.secret_count, dialogueJson.name, dialogueJson.file, dialogueJson.text);
                        DialogueTalkEvents.Add(dialogueTalk);
                        break;
                    case DialogueEventType.CUT_IN:
                        var dialogueCutIn = new DialogueCutInEvent(dialogueJson.event_number, dialogueEventType, dialogueJson.secret_count, dialogueJson.name, dialogueJson.file);
                        DialogueCutInEvents.Add(dialogueCutIn);
                        break;
                    case DialogueEventType.CUT_IN_TALK:
                        var dialogueCutInTalk = new DialogueCutInTalkEvent(dialogueJson.event_number, dialogueEventType, dialogueJson.secret_count, dialogueJson.name, dialogueJson.file, dialogueJson.text);
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

    public void SetRandomlyOrderedTalkEvents()
    {
        var selectedTalkEvents = DialogueTalkEvents
            .Where(dialogueTalkEvent => dialogueTalkEvent.SecretCount == Board.instance.getHowManyHimituDidGet)
            .ToList();
        
        var randomOrder = new Random();
        randomlyOrderedTalkEvents = selectedTalkEvents.OrderBy(x => randomOrder.Next()).ToList();
    }
}
