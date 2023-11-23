using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemainingTurnNumber : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI remainingTurnNumberText;
    private int restTurn;

    // Start is called before the first frame update
    void Start()
    {
        Board.instance.OnChangeRestTurnExecuted += OnChangeRestTurnExecutedHandler;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// c‚èè”‚ğXV
    /// </summary>
    private void OnChangeRestTurnExecutedHandler(int restTurn)
    {
        restTurn = Board.instance.getPresetRestTurn;
        remainingTurnNumberText.text = restTurn.ToString();
    }
}
