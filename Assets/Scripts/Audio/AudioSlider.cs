using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    public Slider seSlider;
    public Slider bgmSlider;
    private AudioSource seSource;
    private AudioSource bgmSource;

    // Start is called before the first frame update
    void Start()
    {
        // Sliderコンポーネントを取得
        seSlider = GameObject.Find("SESlider").GetComponent<Slider>();
        bgmSlider = GameObject.Find("BGMSlider").GetComponent<Slider>();

        AudioSource[] components = this.GetComponents<AudioSource>();
        this.seSource = components[0];
        this.bgmSource = components[1];

        seSlider.value = seSource.volume;
        bgmSlider.value = bgmSource.volume;

        // SEのスライダーの値が変更されたときに音量を更新
        seSlider.onValueChanged.AddListener(UpdateSEVolume);

        // BGMのスライダーの値が変更されたときに音量を更新
        bgmSlider.onValueChanged.AddListener(UpdateBGMVolume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 音量を変更
    private void UpdateSEVolume(float newVolume)
    {
        seSource.volume = newVolume;
        Debug.Log("SEの音量を" + newVolume + "に変更");
    }

    private void UpdateBGMVolume(float newVolume)
    {
        bgmSource.volume = newVolume;
        Debug.Log("BGMの音量を" + newVolume + "に変更");
    }
    
}
