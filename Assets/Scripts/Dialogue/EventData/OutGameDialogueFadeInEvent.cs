public class OutGameDialogueFadeInEvent : AbstractDialogueEvent
{
    private int _eventID;
    public int EventID => _eventID;

    private string _filePath;
    public string FilePath => _filePath;
    public OutGameDialogueFadeInEvent(int eventNumber, DialogueEventType dialogueEventType, int eventID, string filePath) : base(eventNumber, dialogueEventType)
    {
        _eventID = eventID;
        _filePath = Helper.BackgroundPath + filePath;
    }
}
