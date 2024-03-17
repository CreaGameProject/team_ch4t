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
        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - Start()】</color>Before Set Method</b>");

        Board.instance.OnChangeRestTurnExecuted += OnChangeRestTurnExecutedHandler;

        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - Start()】</color>Before Call Method</b>");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 残り手数を更新
    /// </summary>
    private void OnChangeRestTurnExecutedHandler(int restTurn)
    {
        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - OnChangeRestTurnExecutedHandler(int restTurn)】</color>Before Call ChangeRestTurn(" + restTurn + ")</b>");

        restTurn = Board.instance.getPresetRestTurn;
        remainingTurnNumberText.text = restTurn.ToString();

        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - OnChangeRestTurnExecutedHandler(int restTurn)】</color>Before Call ChangeRestTurn(" + restTurn + ")</b>");
    }
}
