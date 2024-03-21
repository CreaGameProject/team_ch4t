using UnityEngine;

public class NewCharacterDialoguePresenter : MonoBehaviour
{
    [SerializeField] private NewCharacterDialogueModel _model;
    [SerializeField] private NewCharacterDialogueView _view;

    private async void Start()
    {
        _view.OnCharacterTalkExecuted += CharacterTalkExecutedEventHandler;

        await _view.StartDialogue();
        _view.ChangeScene();
    }

    private void CharacterTalkExecutedEventHandler(string characterName, string dialogue)
    {
        _model.AddBackLogData(characterName, dialogue);
    }
}
