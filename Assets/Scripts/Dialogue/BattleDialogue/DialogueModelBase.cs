using UnityEngine;

public class DialogueModelBase : MonoBehaviour
{
    private BackLogData _backLogData;
    public BackLogData BackLogData => _backLogData;
    
    private void Start()
    {
        _backLogData = new BackLogData();
    }
    
    public void AddBackLogData(string characterName, string dialogue)
    {
        var logData = new BackLogData.LogData
        {
            speaker = characterName,
            dialogue = dialogue
        };

        _backLogData.logDataList.Add(logData);
        //Debug.Log($"バックログにspeaker: {logData.speaker}, dialogue: {logData.dialogue} を追加しました。");
    }
}
