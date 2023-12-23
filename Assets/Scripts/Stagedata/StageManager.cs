using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager instance_StageManager;
    
    private const int NumStages = 2; // ステージ数
    private int openStageNum = 1;       // 開放されているステージ番号(ゆき１、部長２)
    private bool[] hasCleared = new bool[NumStages + 1];  // 各ステージのクリア状況(ゆき１、部長２)
    private bool[] hasReported = new bool[NumStages + 1]; // 各ステージの報告状況(ゆき１、部長２)
    private int totalReports = 0;   // トータルの報告数

    private void Awake()
    {
        if (instance_StageManager == null)
        {
            instance_StageManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ステージクリア時の処理(ステージ番号, 報告するかどうか)
    public void HandleStageClear(int currentStageNum, bool report)
    {
        if (IsFirstCleared(currentStageNum))
        {
            Debug.Log("ステージ" + currentStageNum + "初回クリア");
            hasCleared[currentStageNum] = true;

            if (report)
            {
                hasReported[currentStageNum] = true;
                totalReports++;
            }
            else
            {
                hasReported[currentStageNum] = false;
            }

            if (currentStageNum == openStageNum)
            {
                openStageNum++;
            }
            SaveData();
        }
        else
        {
            Debug.Log("ステージ" + currentStageNum + "クリア済み");
        }
    }

    // 初回クリアかどうか
    bool IsFirstCleared(int currentStageNum)
    {
        return PlayerPrefs.GetInt("HasCleared" + currentStageNum, 0) == 0;
    }

    // データの保存
    public void SaveData()
    {
        PlayerPrefs.SetInt("OpenStage", openStageNum);
        PlayerPrefs.SetInt("TotalReports", totalReports);
        for (int i = 1; i <= NumStages; i++)
        {
            PlayerPrefs.SetInt("HasCleared" + i, hasCleared[i] ? 1 : 0);     // クリアしている場合は1、していない場合は0
            PlayerPrefs.SetInt("HasReported" + i, hasReported[i] ? 1 : 0);   // レポートがある場合は1、ない場合は0
        }
        PlayerPrefs.Save();

        Debug.Log("トータルの報告数: " + totalReports);
        for (int i = 1; i <= NumStages; i++)
        {
            Debug.Log($"ステージ{i}のクリア状況: " + hasCleared[i]);
            Debug.Log($"ステージ{i}の報告状況: " + hasReported[i]);
        }
    }

    // データの読み込み
    public void LoadData()
    {
        totalReports = PlayerPrefs.GetInt("TotalReports", totalReports);
        for (int i = 1; i <= NumStages; i++)
        {
            hasCleared[i] = PlayerPrefs.GetInt("HasCleared" + i, hasCleared[i] ? 1 : 0) == 1;
            hasReported[i] = PlayerPrefs.GetInt("HasReported" + i, hasReported[i] ? 1 : 0) == 1;
        }
    }

    // 開放されているステージ番号の取得
    public int GetOpenStage()
    {
        return openStageNum = PlayerPrefs.GetInt("OpenStage", openStageNum);
    }

    // 合計報告数の取得
    public int GetTotalReports()
    {
        return totalReports = PlayerPrefs.GetInt("TotalReports", totalReports);
    }

    // クリア状況の取得
    public bool GetHasCleared(int currentStageNum)
    {
        return hasCleared[currentStageNum] = PlayerPrefs.GetInt("HasCleared" + currentStageNum, hasCleared[currentStageNum] ? 1 : 0) == 1;
    }

    // 報告状況の取得
    public bool GetHasReported(int currentStageNum)
    {
        return hasReported[currentStageNum] = PlayerPrefs.GetInt("HasReported" + currentStageNum, hasReported[currentStageNum] ? 1 : 0) == 1;
    }

    // データのリセット
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        openStageNum = 1;
        totalReports = 0;
        for (int i = 1; i <= NumStages; i++)
        {
            hasCleared[i] = false;
            hasReported[i] = false;
        }
    }
}
