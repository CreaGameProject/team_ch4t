using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

public class DialogueViewBase : MonoBehaviour
{
    private int _talkSpeed = 50;
    
    protected Sprite LoadSprite(string filePath)
    {
        return Resources.Load<Sprite>(filePath);
    }
    
    protected async UniTask TypeText(TextMeshProUGUI textMeshProUGUI, string text)
    {
        textMeshProUGUI.text = "";
        string stockString = "";
        bool isStock = false;

        foreach (char c in text)
        {
            if (isStock)
            {
                stockString += c;
                
                if (c == '>')
                {
                    textMeshProUGUI.text += stockString;

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
                    textMeshProUGUI.text += c;
                    AudioManager.instance_AudioManager.PlaySE(4);
                    await UniTask.Delay(_talkSpeed * 5); 
                }
                else if (c == '。' || c == '？' || c == '！' || c == '.')
                {
                    textMeshProUGUI.text += c;
                    AudioManager.instance_AudioManager.PlaySE(4);
                    await UniTask.Delay(_talkSpeed * 10); 
                }
                else
                {
                    textMeshProUGUI.text += c;
                    AudioManager.instance_AudioManager.PlaySE(4);
                    await UniTask.Delay(_talkSpeed); 
                }
            }
        }
    }


    /// <summary>
    /// キャラクターが話終えたときに呼び出される
    /// </summary>
    public delegate void OnCharacterTalkExecutedDelegate(string characterName, string dialogue);

    public event DialogueView.OnCharacterTalkExecutedDelegate OnCharacterTalkExecuted;

    protected void SaveToBackLog(string characterName, string dialogue)
    {
        if (OnCharacterTalkExecuted != null) { OnCharacterTalkExecuted(characterName, dialogue); }
    }
    
    /// <summary>
    /// マウスクリックで次の文章を表示する
    /// </summary>
    protected UniTask WaitUntilMouseClick()
    {
        var clickStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .First()
            .ToUniTask(useFirstValue: true);

        return clickStream;
    }
}