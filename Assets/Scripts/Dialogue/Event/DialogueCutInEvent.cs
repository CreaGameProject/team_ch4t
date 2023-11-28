public class DialogueCutInEvent : AbstractDialogueEvent
{
    private int _secretCount;
    public int SecretCount => _secretCount;
    private string _characterName;
    public string CharacterName => _characterName;
    private string _filePath;
    public string FilePath => _filePath;

    public DialogueCutInEvent(int eventNumber, EventType eventType, int secretCount, string characterName, string filePath)
        : base(eventNumber, eventType)
    {
        _secretCount = secretCount;
        _characterName = characterName;
        _filePath = filePath;
    }
}
