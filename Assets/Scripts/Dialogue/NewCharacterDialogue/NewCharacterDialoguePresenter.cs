using UnityEngine;

public class NewCharacterDialoguePresenter : MonoBehaviour
{
    [SerializeField] private NewCharacterDialogueModel _model;
    [SerializeField] private NewCharacterDialogueView _view;

    private async void Start()
    {
        _view.OnCharacterTalkExecuted += CharacterTalkExecutedEventHandler;
    }

    private void CharacterTalkExecutedEventHandler(string characterName, string dialogue)
    {
        _model.AddBackLogData(characterName, dialogue);
    }
}
