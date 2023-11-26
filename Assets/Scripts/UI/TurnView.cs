using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnView : MonoBehaviour
{
    [SerializeField]
    private Sprite yourTurnImg;
    [SerializeField]
    private Sprite partnerImg;
    private Image turnImage;
    Turn.Type turn;

    // Start is called before the first frame update
    void Start()
    {
        turnImage = GetComponent<Image>();
        Board.instance.OnChangeTurnExecuted += OnChangeTurnExecutedHandler;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 手番（現在のターン）が変更されたときに実行される
    // 引数：type：変更された後の手番（現在のターン）：
    private void OnChangeTurnExecutedHandler(Turn.Type type)
    {
        switch (turn)
        {
            case Turn.Type.player:
                turnImage.sprite = yourTurnImg;
                break;
            case Turn.Type.computer:
                turnImage.sprite = partnerImg;
                break;
            case Turn.Type.neutral:
                break;
        }
    }
}
