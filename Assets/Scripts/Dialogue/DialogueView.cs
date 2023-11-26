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

    public async UniTask StartBattleDialogue(string characterName, string filePath, string dialogue)
    {
        // TODO: イメージはあらかじめ読み込まれている状態にしたい。
        dialogueCharacterImage.sprite = Resources.Load<Sprite>(filePath);
        nameText.text = characterName;
        
        dialogueNextImage.gameObject.SetActive(false);
        await TypeText(dialogue);
        
        dialogueNextImage.gameObject.SetActive(true);
        await WaitUntilMouseClick();
    }
    
    private async UniTask TypeText(string text)
    {
        dialogueText.text = "";

        foreach (char c in text)
        {
            
            // TODO: 話した時の効果音を入れる
            // TODO: 話す速度は変更できるようにする。
            
            if (c == '、')
            {
                dialogueText.text += c;
                await UniTask.Delay(_talkSpeed * 5); 
            }
            else if (c == '。' || c == '？')
            {
                dialogueText.text += c;
                await UniTask.Delay(_talkSpeed * 10); 
            }
            else
            {
                dialogueText.text += c;
                await UniTask.Delay(_talkSpeed); 
            }
        }
    }
    
    private UniTask WaitUntilMouseClick()
    {
        var clickStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .First()
            .ToUniTask(useFirstValue: true);

        return clickStream;
    }
}
