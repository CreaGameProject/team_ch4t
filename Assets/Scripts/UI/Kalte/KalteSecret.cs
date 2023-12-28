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
        public Sprite kalteImage;
        public string birthday;
        public string gradeAndClass;
        public int height;
        public int bodyWeight;
        public string hobby;
        public string specialSkill;
        [TextArea] public string[] secret = new string[3];
    }


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
            "<b>�N���X</b> " + kalteProfile[(int)opponent].gradeAndClass + "\n" +
            "<b>�g��</b> " + kalteProfile[(int)opponent].height + "cm\n" +
            "<b>�̏d</b> " + kalteProfile[(int)opponent].bodyWeight + "kg\n" +
            "<b>�</b> " + kalteProfile[(int)opponent].hobby + "\n" +
            "<b>���Z</b> " + kalteProfile[(int)opponent].specialSkill;
        for (int i = 0; i < 3; i++)
        {
            secretText[i].GetComponent<TextMeshProUGUI>().text = kalteProfile[(int)opponent].secret[i];
            secretText[i].SetActive (false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// �閧���V���ɊJ�������ƌĂ΂��
    /// </summary>
    /// <param name="howManyHimituDidGet">�閧�ԍ�</param>
    private void OnChangeHimituNumberExecutedHandler(int howManyHimituDidGet)
    {
        secretText[howManyHimituDidGet - 1].SetActive(true);
    }
}
