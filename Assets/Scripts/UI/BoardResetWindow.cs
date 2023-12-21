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

        // �E�B���h�E���|�b�v�A�b�v�����鏈��
        GetComponent<PanelControllerNew>()?.OpenPanel();
        Debug.Log("���Z�b�g���܂��I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I");

        while (!popUpWindowLock)
        {
            // 100�~���b�ҋ@���čĎ��s
            await UniTask.Delay(10);
        }

        popUpWindowLock = false;

        GetComponent<PanelControllerNew>()?.ClosePanel();
    }
}
