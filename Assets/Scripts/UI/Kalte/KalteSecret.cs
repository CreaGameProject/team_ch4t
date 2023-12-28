using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class KalteSecret : MonoBehaviour
{
    [System.Serializable]
    class KalteProfile
    {
        [Header("名前")]
        public string name;
        [Header("カルテ画像")]
        public Sprite kalteImage;
        [Header("○月○日(○)")]
        public string birthday;
        [Header("○年○組")]
        public string gradeAndClass;
        [Header("身長")]
        public int height;
        [Header("体重")]
        public int bodyWeight;
        [Header("趣味")]
        public string hobby;
        [Header("特技")]
        public string specialSkill;
        [Header("秘密")]
        [TextArea(3,5)] public string[] secret = new string[3];
    }


    [SerializeField]
    private bool isShowed = false;
    [SerializeField]
    private GameObject image;
    [SerializeField]
    private GameObject birthdayText;
    [SerializeField]
    private GameObject profileText;
    [SerializeField]
    private GameObject[] secretText = new GameObject[3];
    [SerializeField]
    private List<KalteProfile> kalteProfile = new List<KalteProfile>();

    // Start is called before the first frame update
    void Start()
    {
        Board.instance.OnChangeHimituNumberExecuted += OnChangeHimituNumberExecutedHandler;

        Computer.Opponent opponent = Computer.opponent;
        image.GetComponent<Image>().sprite = kalteProfile[(int)opponent].kalteImage;
        birthdayText.GetComponent<TextMeshProUGUI>().text = kalteProfile[(int)opponent].birthday;
        profileText.GetComponent<TextMeshProUGUI>().text =
            "<b>クラス</b> " + kalteProfile[(int)opponent].gradeAndClass + "\n" +
            "<b>身長</b> " + kalteProfile[(int)opponent].height + "cm\n" +
            "<b>体重</b> " + kalteProfile[(int)opponent].bodyWeight + "kg\n" +
            "<b>趣味</b> " + kalteProfile[(int)opponent].hobby + "\n" +
            "<b>特技</b> " + kalteProfile[(int)opponent].specialSkill;
        for (int i = 0; i < 3; i++)
        {
            secretText[i].GetComponent<TextMeshProUGUI>().text = kalteProfile[(int)opponent].secret[i];
            if(!isShowed) secretText[i].SetActive (false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 秘密が新たに開示されると呼ばれる
    /// </summary>
    /// <param name="howManyHimituDidGet">秘密番号</param>
    private void OnChangeHimituNumberExecutedHandler(int howManyHimituDidGet)
    {
        secretText[howManyHimituDidGet - 1].SetActive(true);
    }
}
