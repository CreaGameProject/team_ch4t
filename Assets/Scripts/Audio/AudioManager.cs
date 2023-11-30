using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance_AudioManager;
    
    private void Awake()
    {
        if (instance_AudioManager == null)
        {
            instance_AudioManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private AudioData audioData;
    
    private AudioSource[] seSourceList = new AudioSource[20];
    private AudioSource bgmSource;

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < seSourceList.Length; ++i)
        {
            seSourceList[i] = gameObject.AddComponent<AudioSource>();
        }
        bgmSource = gameObject.AddComponent<AudioSource>();

        CheckOverlap(this.audioData.se_Data, "se_Data");
        CheckOverlap(this.audioData.bgm_Data, "bgm_Data");
    }

    //オーディオIDが重複していないかを確認する
    private void CheckOverlap(List<Datum> data, string variable_name)
    {
        List<int> vs = new List<int>();
        for (int i = 0; i < data.Count; i++)
        {
            if (vs.Contains(data[i].id))
            {
                Debug.LogError(string.Format("{0} のID {1} が重複しています。", variable_name, data[i].id));
            }
            else
            {
                vs.Add(data[i].id);
            }
        }
    }

    //オーディオIDをindexに変換する
    public int ConvertIdIntoIndex(List<Datum> data, int id)
    {
        for (int index = 0; index < data.Count; index++)
        {
            if (id == data[index].id)
            {
                return index;
            }
        }

        Debug.LogError(string.Format("指定されたid {0} のデータは存在しません。", id));

        return -1;
    }
    
    private AudioSource GetUnusedAudioSource() => seSourceList.FirstOrDefault(a => a.isPlaying == false);

    public void PlaySE(int id)
    {
        int index = this.ConvertIdIntoIndex(this.audioData.se_Data, id);
        var seSource = GetUnusedAudioSource();
        seSource.clip = this.audioData.se_Data[index].clip;
        seSource.volume = this.audioData.se_Data[index].volume;
        seSource.Play();
    }

    public void StopSE()
    {
        //this.seSource.Stop();
    }

    public void PauseSE()
    {
        //this.seSource.Pause();
    }

    public void UnPauseSE()
    {
        //this.seSource.UnPause();
    }

    public void PlayBGM(int id)
    {
        int index = this.ConvertIdIntoIndex(this.audioData.bgm_Data, id);
        this.bgmSource.clip = this.audioData.bgm_Data[index].clip;
        this.bgmSource.volume = this.audioData.bgm_Data[index].volume;
        this.bgmSource.Play();
    }

    public void StopBGM()
    {
        this.bgmSource.Stop();
    }

    public void PauseBGM()
    {
        this.bgmSource.Pause();
    }

    public void UnPauseBGM()
    {
        this.bgmSource.UnPause();
    }
}