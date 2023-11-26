using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    public TextMeshProUGUI NameText => nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    public TextMeshProUGUI DialogueText => dialogueText;
    [SerializeField] private Image dialogueCharacterImage;
    public Image DialogueCharacterImage => dialogueCharacterImage;
    [SerializeField] private Image dialogueNextImage;
    public Image DialogueNextImage => dialogueNextImage;

    private int _talkSpeed = 50;
    
    
    
    public void PrefixBattleDialogue(string characterName, string filePath)
    {
        dialogueCharacterImage.sprite = Resources.Load<Sprite>(filePath);
        nameText.text = characterName;
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
}
