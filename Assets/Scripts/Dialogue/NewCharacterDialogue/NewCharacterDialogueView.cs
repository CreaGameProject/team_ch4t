using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterDialogueView : DialogueViewBase
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueNextImage;

    [SerializeField] private SceneChanger _sceneChanger;
    
    private void PrefixDialogue(string characterName)
    {
        dialogueText.text = "";
    }

    public async UniTask StartDialogue()
    {
        var dialogue = "新しいキャラクターが解放されました。";
        
        WaitForClick().Forget();
        await TypeText(dialogueText, dialogue);
        SaveToBackLog("", dialogue);
            
        dialogueNextImage.gameObject.SetActive(true);
        await WaitUntilMouseClick();
        dialogueNextImage.gameObject.SetActive(false);
    }

    public void ChangeScene()
    {
        _sceneChanger.LoadScene("Title");
    }
}