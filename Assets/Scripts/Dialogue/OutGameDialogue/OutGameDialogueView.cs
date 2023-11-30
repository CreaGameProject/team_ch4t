using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

public class OutGameDialogueView : MonoBehaviour
{
    private int _talkSpeed = 50;

    private Sprite LoadSprite(string filePath)
    {
        return Resources.Load<Sprite>(filePath);
    }

    private async UniTask TypeText(TextMeshProUGUI textMeshProUGUI, string text)
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

    public event DialogueView.OnCharacterTalkExecutedDelegate OnCharacterTalkExecuted;

    private void SaveToBackLog(string characterName, string dialogue)
    {
        if (OnCharacterTalkExecuted != null) { OnCharacterTalkExecuted(characterName, dialogue); }
    }

    /// <summary>
    /// マウスクリックで次の文章を表示する
    /// </summary>
    private UniTask WaitUntilMouseClick()
    {
        var clickStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .First()
            .ToUniTask(useFirstValue: true);

        return clickStream;
    }
}
