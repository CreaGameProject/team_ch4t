using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelecter : MonoBehaviour
{
    [Header("キャラクター")]
    [SerializeField]
    Computer.Opponent opponent;
    [SerializeField]
    Button button;
    //[SerializeField]
    //TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnClick);
        //text.text = opponent.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClick()
    {
        // 対戦相手を設定
        Computer.opponent = opponent;

        SceneManager.LoadScene("Dialogue");
    }
}
