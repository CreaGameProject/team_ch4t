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
    /// �c��萔��1����������
    /// </summary>
    public void ReduceRemainingTurns()
    {

    }
}
