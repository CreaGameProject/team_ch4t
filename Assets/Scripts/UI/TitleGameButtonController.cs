using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleGameButtonController : MonoBehaviour
{
    [SerializeField] private GameObject startButton = null;
    [SerializeField] private GameObject exitButton = null;

    // WebGL用のボタンの座標
    [SerializeField] private Transform webGLStartButtonPosition = null;

    private void Awake()
    {

#if UNITY_WEBGL
        this.startButton.transform.position = this.webGLStartButtonPosition.position;
        this.startButton.SetActive(true);

        this.exitButton.SetActive(false);
#endif

    }
}
