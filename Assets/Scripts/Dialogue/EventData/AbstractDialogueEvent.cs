public abstract class AbstractDialogueEvent
{
    private int _eventNumber;
    public int EventNumber => _eventNumber;
    private DialogueEventType _dialogueEventType;
    public DialogueEventType DialogueEventType => _dialogueEventType;

    protected AbstractDialogueEvent(int eventNumber, DialogueEventType dialogueEventType)
    {
        _eventNumber = eventNumber;
        _dialogueEventType = dialogueEventType;
    }
}
