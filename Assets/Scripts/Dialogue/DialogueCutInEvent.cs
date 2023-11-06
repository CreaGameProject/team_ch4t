public class DialogueCutInEvent : AbstractDialogueEvent
{
    private string _characterName;
    public string CharacterName => _characterName;
    private string _filePath;
    public string FilePath => _filePath;
    private string _text;
    public string Text => _text;
    
    public DialogueCutInEvent(int eventNumber, EventType eventType, string characterName, string filePath, string text)
        : base(eventNumber, eventType)
    {
        _characterName = characterName;
        _filePath = filePath;
        _text = text;
    }
}
