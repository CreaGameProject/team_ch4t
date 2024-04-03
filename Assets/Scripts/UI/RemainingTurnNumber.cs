using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemainingTurnNumber : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI remainingTurnNumberText;

    private void Awake()
    {
        //remainingTurnNumberText.text = "?";
        //Debug.Log("手数を更新する");

        /*
        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - Start()】</color>Before Set Method</b>");
        Board.instance.OnChangeRestTurnExecuted += OnChangeRestTurnExecutedHandler;
        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - Start()】</color>After Set Method</b>");
        */
    }

    private void OnEnable()
    {
        /*
        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - Start()】</color>Before Set Method</b>");
        Board.instance.OnChangeRestTurnExecuted += OnChangeRestTurnExecutedHandler;
        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - Start()】</color>After Set Method</b>");
        */
    }

    private void Reset()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - Start()】</color>Before Set Method</b>");
        Board.instance.OnChangeRestTurnExecuted += OnChangeRestTurnExecutedHandler;
        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - Start()】</color>After Set Method</b>");
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 残り手数を更新
    /// </summary>
    public void OnChangeRestTurnExecutedHandler(int restTurn)
    {
        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - OnChangeRestTurnExecutedHandler(int restTurn)】</color>Before Call ChangeRestTurn(" + restTurn + ")</b>");

        restTurn = Board.instance.getPresetRestTurn;
        remainingTurnNumberText.text = restTurn.ToString();

        Debug.Log("<b><color=#f35b04>【RemainingTurnNumber - OnChangeRestTurnExecutedHandler(int restTurn)】</color>After Call ChangeRestTurn(" + restTurn + ")</b>");
    }
}
