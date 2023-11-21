using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 報告ボタンなどにアタッチしてください
    public void AddReports()
    {
        int Reports = PlayerPrefs.GetInt("Reports");
        Reports++;
        PlayerPrefs.SetInt("Reports", Reports);
        Debug.Log("現在のレポート数" + PlayerPrefs.GetInt("Reports"));
    }
}
