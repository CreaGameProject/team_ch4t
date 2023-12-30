using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioSlider : MonoBehaviour
{
    private Slider seSlider;
    private Slider bgmSlider;
    private AudioSource seSource;
    private AudioSource bgmSource;

    GameObject audioManager = null;

    // Start is called before the first frame update
    void Start()
    {
        seSlider = GameObject.Find("SESlider").GetComponent<Slider>();
        bgmSlider = GameObject.Find("BGMSlider").GetComponent<Slider>();

        Debug.Log("aaaaaaaaaaaaaaaa");

        // AudioManagerオブジェクトを取得
        audioManager = GameObject.Find("AudioManager");
        
        // AudioManagerが存在し、AudioSourceが2つ以上あるか確認
        if (audioManager != null)
        {
            AudioSource[] components = audioManager.GetComponents<AudioSource>();

            // AudioSourceが2つ以上ある場合にのみ代入
            if (components.Length >= 2)
            {
                this.bgmSource = components[0];
                this.seSource = components[1];
                

                seSlider.value = seSource.volume;
                bgmSlider.value = bgmSource.volume;

                // SEのスライダーの値が変更されたときに音量を更新
                seSlider.onValueChanged.AddListener(UpdateSEVolume);

                // BGMのスライダーの値が変更されたときに音量を更新
                bgmSlider.onValueChanged.AddListener(UpdateBGMVolume);
            }
            else
            {
                Debug.LogError("AudioManagerに2つ以上のAudioSourceが必要です。");
            }
        }
        else
        {
            Debug.LogError("AudioManagerが見つかりません。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 音量を変更
    private void UpdateSEVolume(float newVolume)
    {
        audioManager.GetComponent<AudioManager>().SetSEVolume(newVolume);
        seSource.volume = newVolume;
        Debug.Log("SEの音量を" + newVolume + "に変更");
    }

    private void UpdateBGMVolume(float newVolume)
    {
        bgmSource.volume = newVolume;
        Debug.Log("BGMの音量を" + newVolume + "に変更");
    }
    
}
