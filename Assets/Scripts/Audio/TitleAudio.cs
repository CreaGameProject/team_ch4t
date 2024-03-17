using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance_AudioManager.StopBGM();
        AudioManager.instance_AudioManager.PlayBGM(3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
