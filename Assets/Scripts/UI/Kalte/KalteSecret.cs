using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class KalteSecret : MonoBehaviour
{
    [SerializeField]
    private KalteProfile kalteProfile;
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

    // Start is called before the first frame update
    void Start()
    {
        Computer.Opponent opponent = Computer.opponent;
        image.GetComponent<Image>().sprite = kalteProfile.profiles[(int)opponent].kalteImage;
        birthdayText.GetComponent<TextMeshProUGUI>().text = kalteProfile.profiles[(int)opponent].birthday;
        profileText.GetComponent<TextMeshProUGUI>().text =
            "<b>�N���X</b> " + kalteProfile.profiles[(int)opponent].gradeAndClass + "\n" +
            "<b>�g��</b> " + kalteProfile.profiles[(int)opponent].height + "cm\n" +
            "<b>�̏d</b> " + kalteProfile.profiles[(int)opponent].bodyWeight + "kg\n" +
            "<b>�</b> " + kalteProfile.profiles[(int)opponent].hobby + "\n" +
            "<b>���Z</b> " + kalteProfile.profiles[(int)opponent].specialSkill;
        for (int i = 0; i < 3; i++)
        {
            secretText[i].GetComponent<TextMeshProUGUI>().text = "<b>�Ђ݂�" + (i+1) +" </b>" + kalteProfile.profiles[(int)opponent].secret[i];
            if (!isShowed)
            {
                Board.instance.OnChangeHimituNumberExecuted += OnChangeHimituNumberExecutedHandler;
                secretText[i].SetActive(false);
            }
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
