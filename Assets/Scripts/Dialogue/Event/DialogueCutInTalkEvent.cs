public class DialogueCutInTalkEvent: AbstractDialogueEvent
{
    private int _secretCount;
    public int SecretCount => _secretCount;
    private string _characterName;
    public string CharacterName => _characterName;
    private string _filePath;
    public string FilePath => _filePath;
    private string _text;
    public string Text => _text;

    public DialogueCutInTalkEvent(int eventNumber, EventType eventType, int secretCount, string characterName, string filePath, string text)
        : base(eventNumber, eventType)
    {
        _secretCount = secretCount;
        _characterName = characterName;
        _filePath = filePath;
        _text = text;
    }
}
