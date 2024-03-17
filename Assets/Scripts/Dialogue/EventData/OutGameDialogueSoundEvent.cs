public class OutGameDialogueSoundEvent : AbstractDialogueEvent
{
    private int _eventID;
    public int EventID => _eventID;

    private string _filePath;
    public string FilePath => _filePath;
    private string _text;
    public string Text => _text;
    public OutGameDialogueSoundEvent(int eventNumber, DialogueEventType dialogueEventType, int eventID, string filePath, string text) : base(eventNumber, dialogueEventType)
    {
        _eventID = eventID;
        _filePath = filePath;
        _text = text;
    }
}
