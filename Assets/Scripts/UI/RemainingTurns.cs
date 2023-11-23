using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemainingTurns : MonoBehaviour
{
    [SerializeField]
    private GameObject remainingTurns;
    private TextMeshProUGUI remainingTurnsText;

    // Start is called before the first frame update
    void Start()
    {
        remainingTurnsText = remainingTurns.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 残り手数を1だけ減じる
    /// </summary>
    public void ReduceRemainingTurns()
    {

    }
}
