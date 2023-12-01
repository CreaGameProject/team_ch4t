using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutGameDialogueView : DialogueViewBase
{
    [SerializeField] private Image background;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject itemBackground;
    [SerializeField] private Image itemImage;
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image characterImageLeft;
    [SerializeField] private Image characterImageCenter;
    [SerializeField] private Image characterImageRight;
    [SerializeField] private Image dialogueNextImage;
    
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;

    public async UniTask StartDialogue(string characterName, string filePath, string characterNameSub, string filePathSub, int speaker, string dialogue)
    {
        if (characterNameSub == "")
        {
            await StartSoloDialogue(characterName, filePath, dialogue);
        }
        else
        {
            await StartDuoDialogue(characterName, filePath, characterNameSub, filePathSub, speaker, dialogue);
        }
    }

    private async UniTask StartDuoDialogue(string characterName, string filePath, string characterNameSub, string filePathSub, int speaker,string dialogue)
    {
        characterImageLeft.gameObject.SetActive(filePath != Helper.CharacterFilePath);
        characterImageRight.gameObject.SetActive(filePathSub != Helper.CharacterFilePath);
        characterImageCenter.gameObject.SetActive(false);
        
        characterImageLeft.sprite = LoadSprite(filePath);
        characterImageRight.sprite = LoadSprite(filePathSub);
        
        if (speaker == 0)
        {
            PrefixDialogue(characterName);
            
            characterImageLeft.color = activeColor;
            characterImageRight.color = inactiveColor;
            
            await TypeText(dialogueText, dialogue);
            SaveToBackLog(characterName, dialogue);
            
            dialogueNextImage.gameObject.SetActive(true);
            await WaitUntilMouseClick();
            dialogueNextImage.gameObject.SetActive(false);
        }
        else
        {
            PrefixDialogue(characterNameSub);
            
            characterImageLeft.color = inactiveColor;
            characterImageRight.color = activeColor;
            
            await TypeText(dialogueText, dialogue);
            SaveToBackLog(characterNameSub, dialogue);
            
            dialogueNextImage.gameObject.SetActive(true);
            await WaitUntilMouseClick();
            dialogueNextImage.gameObject.SetActive(false);
        }
    }

    private async UniTask StartSoloDialogue(string characterName, string filePath, string dialogue)
    {
        characterImageLeft.gameObject.SetActive(false);
        characterImageRight.gameObject.SetActive(false);

        characterImageCenter.gameObject.SetActive(filePath != Helper.CharacterFilePath);

        PrefixDialogue(characterName);
        
        characterImageCenter.sprite = LoadSprite(filePath);
        await TypeText(dialogueText, dialogue);
        SaveToBackLog(characterName, dialogue);
        
        dialogueNextImage.gameObject.SetActive(true);
        await WaitUntilMouseClick();
        dialogueNextImage.gameObject.SetActive(false);
    }
    
    private void PrefixDialogue(string characterName)
    {
        dialogueText.text = "";
        nameText.text = characterName;
    }

    public async UniTask ShowItem(string filePath)
    {
        itemBackground.SetActive(true);
        itemImage.sprite = LoadSprite(filePath);
        itemImage.gameObject.SetActive(true);
        AudioManager.instance_AudioManager.PlaySE(0);
        
        await WaitUntilMouseClick();
        
        itemBackground.SetActive(false);
        itemImage.gameObject.SetActive(false);
    }

    public async UniTask PlaySound(string filePath, string dialogue)
    {
        PrefixDialogue("");
        AudioManager.instance_AudioManager.PlaySE(int.Parse(filePath));
        await TypeText(dialogueText, dialogue);
        await WaitUntilMouseClick();
        SaveToBackLog("", dialogue);
    }

    public async UniTask PlayFadeIn(string filePath, CancellationToken token)
    {
        fadeImage.gameObject.SetActive(true);
        background.sprite = LoadSprite(filePath);
        await fadeImage.DOFade(0.0f, 1f)
            .OnComplete(() => {fadeImage.gameObject.SetActive(false);})
            .ToUniTask(cancellationToken: token);
    }

    public async UniTask PlayFadeOut(CancellationToken token)
    {
        fadeImage.gameObject.SetActive(true);
        await fadeImage.DOFade(1.0f, 1f)
            .ToUniTask(cancellationToken: token);
    }
    
    /// <summary>
    /// EventType.ENDが実行完了したときに呼ばれる
    /// </summary>
    public delegate void OnDialogueEndDelegate();

    public event OnDialogueEndDelegate OnDialogueEnd;

    public void OnEnd()
    {
        if (OnDialogueEnd != null) { OnDialogueEnd(); }
    }
}
