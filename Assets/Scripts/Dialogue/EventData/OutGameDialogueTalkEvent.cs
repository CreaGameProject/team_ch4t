public class OutGameDialogueTalkEvent : AbstractDialogueEvent
{
    private int _eventID;
    public int EventID => _eventID;
    private string _characterName;
    public string CharacterName => _characterName;
    private string _filePath;
    public string FilePath => _filePath;
    private string _characterNameSub;
    public string CharacterNameSub => _characterNameSub;
    private string _filePathSub;
    public string FilePathSub => _filePathSub;
    private int _talkerNumber;
    public int TalkerNumber => _talkerNumber;
    private string _text;
    public string Text => _text;

    public OutGameDialogueTalkEvent(
        int eventNumber,
        DialogueEventType dialogueEventType,
        int eventID,
        string characterName,
        string filePath,
        string characterNameSub,
        string filePathSub,
        int talkerNumber,
        string text
    ) : base(eventNumber, dialogueEventType)
    {
        _eventID = eventID;
        _characterName = characterName;
        _filePath = Helper.CharacterFilePath + filePath;
        _characterNameSub = characterNameSub;
        _filePathSub = Helper.CharacterFilePath + filePathSub;
        _talkerNumber = talkerNumber;
        _text = text;
    }
}
