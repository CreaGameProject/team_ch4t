using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPressedButtonSe()
    {
        AudioManager.instance_AudioManager.PlaySE(0);
    }

    public void PlayHighlightedButtonSe()
    {
        AudioManager.instance_AudioManager.PlaySE(1);
    }
}
