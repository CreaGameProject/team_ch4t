using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardResetWindow : MonoBehaviour
{
    public bool popUpWindowLock = false;
    [SerializeField]
    private Button yesButton;

    // Start is called before the first frame update
    void Start()
    {
        Board.instance.ImpossiblePlaceStonesExecuted += ImpossiblePlaceStones;
        yesButton.onClick.AddListener(() => popUpWindowLock = true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async public UniTask ImpossiblePlaceStones()
    {
        await UniTask.Yield();

        // ウィンドウをポップアップさせる処理
        GetComponent<PanelControllerNew>()?.OpenPanel();

        while (!popUpWindowLock)
        {
            // 100ミリ秒待機して再試行
            await UniTask.Delay(10);
        }

        popUpWindowLock = false;

        GetComponent<PanelControllerNew>()?.ClosePanel();
    }
}
