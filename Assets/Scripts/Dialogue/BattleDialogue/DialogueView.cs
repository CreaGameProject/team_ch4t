using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : DialogueViewBase
{
    [Header("BattleDialogue")]
    [SerializeField] private GameObject battleDialogueUI;
    [SerializeField] private TextMeshProUGUI battleNameText;
    [SerializeField] private TextMeshProUGUI battleDialogueText;
    [SerializeField] private Image battleCharacterImage;

    private int _talkSpeed = 50;
    
    [Header("CutIn")]
    [SerializeField] private GameObject cutInUI;
    [SerializeField] private Image cutInCharacterImage;
    [SerializeField] private Image cutInBackgroundMask;
    
    [Header("CutInTalk")]
    [SerializeField] private GameObject cutInTalkUI;
    [SerializeField] private TextMeshProUGUI cutInTalkNameText;
    [SerializeField] private TextMeshProUGUI cutInTalkDialogueText;
    [SerializeField] private Image cutInTalkCharacterImage;
    [SerializeField] private Image cutInTalkNextImage;
    

    public async UniTask StartBattleDialogue(string characterName, string filePath, string dialogue)
    {
        // TODO: イメージはあらかじめ読み込まれている状態にしたい。
        PrefixBattleDialogue(characterName, filePath);

        WaitForClick().Forget();
        await TypeText(battleDialogueText, dialogue);
        SaveToBackLog(characterName, dialogue);
    }
    
    public void PrefixBattleDialogue(string characterName, string filePath)
    {
        battleDialogueUI.SetActive(true);
        battleCharacterImage.sprite = LoadSprite(filePath);
        battleNameText.text = characterName;
    }

    public async UniTask PrefixCutInTalkDialogue(string characterName, string filePath)
    {
        await UniTask.Delay(500);
        
        battleDialogueText.text = "";
        
        cutInTalkUI.SetActive(true);
        if (filePath == Helper.CharacterFilePath)
        {
            cutInTalkCharacterImage.gameObject.SetActive(false);
        }
        else
        {
            cutInTalkCharacterImage.gameObject.SetActive(true);
            cutInTalkCharacterImage.sprite = LoadSprite(filePath);
        }
        cutInTalkNameText.text = characterName;
        
        cutInTalkNextImage.gameObject.SetActive(false);
        battleDialogueUI.SetActive(false);
    }
    
    public async UniTask StartCutInTalkDialogue(string characterName, string filePath, string dialogue)
    {
        if (filePath == Helper.CharacterFilePath)
        {
            cutInTalkCharacterImage.gameObject.SetActive(false);
        }
        else
        {
            cutInTalkCharacterImage.gameObject.SetActive(true);
            cutInTalkCharacterImage.sprite = LoadSprite(filePath);
            battleCharacterImage.sprite = LoadSprite(filePath); // カットイン終了後画像を同期させるために
        }

        cutInTalkNameText.text = characterName;
        
        WaitForClick().Forget();
        await TypeText(cutInTalkDialogueText, dialogue);
        SaveToBackLog(characterName, dialogue);
        
        cutInTalkNextImage.gameObject.SetActive(true);
        await WaitUntilMouseClick();
        
        cutInTalkNextImage.gameObject.SetActive(false);
    }

    public void CloseCutInTalkDialogue()
    {
        cutInTalkUI.SetActive(false);
        battleDialogueUI.SetActive(true);
    }

    public async UniTask StartCutIn(string filePath, CancellationToken token)
    {
        var backgroundRect = cutInBackgroundMask.GetComponent<RectTransform>();
        var characterRect = cutInCharacterImage.GetComponent<RectTransform>();
        cutInUI.SetActive(true);
        cutInCharacterImage.sprite = LoadSprite(filePath);
        
        AudioManager.instance_AudioManager.PlaySE(5);

        await UniTask.WhenAll(
            characterRect.DOAnchorPos(new Vector2(1500, 0), 0.0f).ToUniTask(cancellationToken: token),
            backgroundRect.DOAnchorPos(new Vector2(300, 375), 0.0f).ToUniTask(cancellationToken: token),
            backgroundRect.DOSizeDelta(new Vector2(0, 2500), 0.0f).ToUniTask(cancellationToken: token)
        );

        await UniTask.WhenAll(
            characterRect.DOAnchorPos(new Vector2(-400, 0), 0.5f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token),
            backgroundRect.DOSizeDelta(new Vector2(2000, 2500), 0.5f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token)
        );

        await UniTask.Delay(700);

        await UniTask.WhenAll(
            characterRect.DOAnchorPos(new Vector2(-1500, 0), 0.3f).SetEase(Ease.InCubic)
                .ToUniTask(cancellationToken: token),
            backgroundRect.DOAnchorPos(new Vector2(-1500, 375), 0.3f).SetEase(Ease.InCubic)
                .OnComplete(() => { cutInUI.SetActive(false); })
                .ToUniTask(cancellationToken: token)
        );
    }
}
