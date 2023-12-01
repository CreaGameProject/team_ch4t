public class OutGameDialogueEndEvent : AbstractDialogueEvent
{
    private int _eventID;
    public int EventID => _eventID;
    public OutGameDialogueEndEvent(int eventNumber, DialogueEventType dialogueEventType, int eventID) : base(eventNumber, dialogueEventType)
    {
        _eventID = eventID;
    }
}
