public class OutGameDialogueFadeOutEvent : AbstractDialogueEvent
{
    private int _eventID;
    public int EventID => _eventID;
    
    public OutGameDialogueFadeOutEvent(int eventNumber, DialogueEventType dialogueEventType, int eventID) : base(eventNumber, dialogueEventType)
    {
        _eventID = eventID;
    }
}
