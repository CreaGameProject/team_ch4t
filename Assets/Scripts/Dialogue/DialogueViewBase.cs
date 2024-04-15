using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class DialogueViewBase : MonoBehaviour
{
    [SerializeField] private int _talkSpeed = 50;
    private bool isSkip = false;
    
    protected Sprite LoadSprite(string filePath)
    {
        return Resources.Load<Sprite>(filePath);
    }
    
    protected async UniTask TypeText(TextMeshProUGUI textMeshProUGUI, string text)
    {
        textMeshProUGUI.text = "";
        string stockString = "";
        bool isStock = false;
        isSkip = false;
        
        foreach (char c in text)
        {
            // isSkip = await WaitForClick().Forget();
            
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
                    
                    if (!isSkip)
                    {
                        AudioManager.instance_AudioManager.PlaySE(4);
                        await UniTask.Delay(_talkSpeed * 5);
                    }
                }
                else if (c == '。' || c == '？' || c == '！' || c == '.' || c == '…')
                {
                    textMeshProUGUI.text += c;
                    
                    if (!isSkip)
                    {
                        AudioManager.instance_AudioManager.PlaySE(4);
                        await UniTask.Delay(_talkSpeed * 10);
                    }
                }
                else
                {
                    textMeshProUGUI.text += c;
                    
                    if (!isSkip)
                    {
                        AudioManager.instance_AudioManager.PlaySE(4);
                        await UniTask.Delay(_talkSpeed);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// キャラクターが話終えたときに呼び出される
    /// </summary>
    public delegate void OnCharacterTalkExecutedDelegate(string characterName, string dialogue);

    public event OnCharacterTalkExecutedDelegate OnCharacterTalkExecuted;

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
            .Where(_ => Helper.isAllowedTextClick)
            .Where(_ => Input.GetMouseButtonDown(0))
            .First()
            .ToUniTask(useFirstValue: true);

        return clickStream;
    }
    
    public async UniTask WaitForClick()
    {
        // UniTaskCompletionSourceを使用して非同期処理を管理
        var tcs = new UniTaskCompletionSource<bool>();

        // クリックを検知する非同期処理
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Helper.isAllowedTextClick)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        // クリックが検知されたらtrueを設定して完了
                        tcs.TrySetResult(true);
                    }
                }
            });

        // クリックが検知されるまで待機
        isSkip = await tcs.Task;
    }
    
}