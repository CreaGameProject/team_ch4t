public abstract class AbstractDialogueEvent
{
    private int _eventNumber;
    public int EventNumber => _eventNumber;
    private EventType _eventType;
    public EventType EventType => _eventType;

    protected AbstractDialogueEvent(int eventNumber, EventType eventType)
    {
        _eventNumber = eventNumber;
        _eventType = eventType;
    }
}
