using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalteManager : MonoBehaviour
{
    // ================================================
    // このスクリプトは、いずれ消去予定
    // ================================================

    [SerializeField]
    private GameObject[] kaltes = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(kaltes[0], GetComponent<RectTransform>());

        Computer.Opponent opponent = Computer.opponent;
        switch (opponent)
        {
            case Computer.Opponent.Yukihira_ui:
                Instantiate(kaltes[0], GetComponent<RectTransform>());
                break;
            case Computer.Opponent.Takahashi_shota:
                Instantiate(kaltes[1], GetComponent<RectTransform>());
                break;
        }
    }
}
