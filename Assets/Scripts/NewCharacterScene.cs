using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewCharacterScene : MonoBehaviour
{
    [SerializeField]
    private Button nextSceneButton;

    // Start is called before the first frame update
    void Start()
    {
        nextSceneButton.onClick.AddListener(() => SceneManager.LoadScene("Title"));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
