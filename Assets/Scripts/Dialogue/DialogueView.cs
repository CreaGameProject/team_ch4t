using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [Header("BattleDialogue")]
    [SerializeField] private GameObject battleDialogueUI;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image battleCharacterImage;
    [SerializeField] private Image dialogueNextImage;

    private int _talkSpeed = 50;
    
    [Header("CutIn")]
    [SerializeField] private GameObject cutInUI;
    [SerializeField] private Image cutInCharacterImage;
    [SerializeField] private Image cutInBackgroundMask;
    
    
    
    public void PrefixBattleDialogue(string characterName, string filePath)
    {
        battleDialogueUI.SetActive(true);
        battleCharacterImage.sprite = LoadSprite(filePath);
        nameText.text = characterName;
    }

    private Sprite LoadSprite(string filePath)
    {
        return Resources.Load<Sprite>(filePath);
    }

    public async UniTask StartBattleDialogue(string characterName, string filePath, string dialogue)
    {
        // TODO: イメージはあらかじめ読み込まれている状態にしたい。
        PrefixBattleDialogue(characterName, filePath);
        
        dialogueNextImage.gameObject.SetActive(false);
        await TypeText(dialogue);
        
        //dialogueNextImage.gameObject.SetActive(true);
        //await WaitUntilMouseClick();
    }
    
    private async UniTask TypeText(string text)
    {
        dialogueText.text = "";
        string stockString = "";
        bool isStock = false;

        foreach (char c in text)
        {
            if (isStock)
            {
                stockString += c;
                
                if (c == '>')
                {
                    dialogueText.text += stockString;

                    stockString = "";
                    isStock = false;
                }
            }
            else
            {
                if (c == '<')
                {
                    stockString += c;
                    isStock = true;
                }
                else if (c == '、')
                {
                    dialogueText.text += c;
                    // TODO: 話した時の効果音を入れる
                    await UniTask.Delay(_talkSpeed * 5); 
                }
                else if (c == '。' || c == '？')
                {
                    dialogueText.text += c;
                    // TODO: 話した時の効果音を入れる
                    await UniTask.Delay(_talkSpeed * 10); 
                }
                else
                {
                    dialogueText.text += c;
                    // TODO: 話した時の効果音を入れる
                    await UniTask.Delay(_talkSpeed); 
                }
            }
            
            
        }
    }
    
    // マウスクリックで次の文章を表示する
    private UniTask WaitUntilMouseClick()
    {
        var clickStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .First()
            .ToUniTask(useFirstValue: true);

        return clickStream;
    }

    public async UniTask StartCutIn(string filePath, CancellationToken token)
    {
        var backgroundRect = cutInBackgroundMask.GetComponent<RectTransform>();
        var characterRect = cutInCharacterImage.GetComponent<RectTransform>();
        cutInUI.SetActive(true);

        await UniTask.WhenAll(
            characterRect.DOAnchorPos(new Vector2(1500, -150), 0.0f).ToUniTask(cancellationToken: token),
            backgroundRect.DOAnchorPos(new Vector2(200, 375), 0.0f).ToUniTask(cancellationToken: token),
            backgroundRect.DOSizeDelta(new Vector2(0, 2500), 0.0f).ToUniTask(cancellationToken: token)
        );

        await UniTask.WhenAll(
            characterRect.DOAnchorPos(new Vector2(-600, -150), 0.5f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token),
            backgroundRect.DOSizeDelta(new Vector2(1500, 2500), 0.5f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token)
        );

        await UniTask.Delay(700);

        await UniTask.WhenAll(
            characterRect.DOAnchorPos(new Vector2(-1500, -150), 0.3f).SetEase(Ease.InCubic)
                .ToUniTask(cancellationToken: token),
            backgroundRect.DOAnchorPos(new Vector2(-1500, 375), 0.3f).SetEase(Ease.InCubic)
                .OnComplete(() => { cutInUI.SetActive(false); })
                .ToUniTask(cancellationToken: token)
        );
    }
}
