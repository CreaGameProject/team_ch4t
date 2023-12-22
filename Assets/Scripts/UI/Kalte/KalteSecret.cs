using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalteSecret : MonoBehaviour
{
    [SerializeField]
    private GameObject[] secretText = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        Board.instance.OnChangeHimituNumberExecuted += OnChangeHimituNumberExecutedHandler;

        for (int i = 0; i < 3; i++)
        {
            secretText[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void AddNextSecretText(int secret)
    {
        secretText[secret].SetActive(true);
    }

    private void OnChangeHimituNumberExecutedHandler(int howManyHimituDidGet)
    {
        AddNextSecretText(howManyHimituDidGet - 1);
    }
}
