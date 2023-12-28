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
        [Header("���O")]
        public string name;
        [Header("�J���e�摜")]
        public Sprite kalteImage;
        [Header("��������(��)")]
        public string birthday;
        [Header("���N���g")]
        public string gradeAndClass;
        [Header("�g��")]
        public int height;
        [Header("�̏d")]
        public int bodyWeight;
        [Header("�")]
        public string hobby;
        [Header("���Z")]
        public string specialSkill;
        [Header("�閧")]
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
            "<b>�N���X</b> " + kalteProfile[(int)opponent].gradeAndClass + "\n" +
            "<b>�g��</b> " + kalteProfile[(int)opponent].height + "cm\n" +
            "<b>�̏d</b> " + kalteProfile[(int)opponent].bodyWeight + "kg\n" +
            "<b>�</b> " + kalteProfile[(int)opponent].hobby + "\n" +
            "<b>���Z</b> " + kalteProfile[(int)opponent].specialSkill;
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
    /// �閧���V���ɊJ�������ƌĂ΂��
    /// </summary>
    /// <param name="howManyHimituDidGet">�閧�ԍ�</param>
    private void OnChangeHimituNumberExecutedHandler(int howManyHimituDidGet)
    {
        secretText[howManyHimituDidGet - 1].SetActive(true);
    }
}
