public class DialogueTalkEvent : AbstractDialogueEvent
{
    private int _secretCount;
    public int SecretCount => _secretCount;
    private string _characterName;
    public string CharacterName => _characterName;
    private string _filePath;
    public string FilePath => _filePath;
    private string _text;
    public string Text => _text;

    public DialogueTalkEvent(int eventNumber, DialogueEventType dialogueEventType, int secretCount, string characterName, string filePath, string text)
        : base(eventNumber, dialogueEventType)
    {
        _secretCount = secretCount;
        _characterName = characterName;
        _filePath = Helper.CharacterFilePath + filePath;
        _text = text;
    }
}
