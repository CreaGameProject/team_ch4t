public class DialogueCutInEvent : AbstractDialogueEvent
{
    private string _characterName;
    public string CharacterName => _characterName;
    private string _filePath;
    public string FilePath => _filePath;

    public DialogueCutInEvent(int eventNumber, EventType eventType, string characterName, string filePath)
        : base(eventNumber, eventType)
    {
        _characterName = characterName;
        _filePath = filePath;
    }
}
