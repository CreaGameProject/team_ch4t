using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private const int NumStages = 2; // ステージ数

    private int currentStageNumber = 1; // 今のステージ番号
    private int totalReports = 0;   // トータルの報告数
    private bool[] hasReported = new bool[NumStages + 1]; // 各ステージの報告状況

    private const string CurrentStageKey = "CurrentStage";
    private const string TotalReportsKey = "TotalReports";
    private const string IsFirstClearKey = "IsFirstClear";

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
        if (IsFirstCleared())
        {
            if (report)
            {
                hasReported[currentStageNum] = true;
                totalReports++;
            }
            else
            {
                hasReported[currentStageNum] = false;
            }
            SaveData();
        }
        else
        {

        }
    }

    // 初回クリアかどうか
    bool IsFirstCleared()
    {
        return PlayerPrefs.GetInt(GetIsFirstClearKey(currentStageNumber), 0) == 1;
    }

    // データの保存
    public void SaveData()
    {
        PlayerPrefs.SetInt(CurrentStageKey, currentStageNumber);
        PlayerPrefs.SetInt(TotalReportsKey, totalReports);
        PlayerPrefs.SetInt(GetIsFirstClearKey(currentStageNumber), 1);
        PlayerPrefs.Save();

        Debug.Lod("現在のステージ番号: " + currentStageNumber);
        Debug.Lod("トータルの報告数: " + totalReports);
        for (int i = 0; i < NumStages; i++)
        {
            Debug.Lod($"ステージ{i + 1}の報告状況: " + hasReported[i]);
        }
    }

    // データの読み込み
    public void LoadData()
    {
        currentStageNumber = PlayerPrefs.GetInt(CurrentStageKey, currentStageNumber);
        totalReports = PlayerPrefs.GetInt(TotalReportsKey, totalReports);
        for (int i = 0; i < NumStages; i++)
        {
            hasReported[i] = PlayerPrefs.GetInt(GetIsFirstClearKey(i + 1), 0) == 1;
        }
    }

    // IsFirstClearKeyの取得
    private string GetIsFirstClearKey(int stageNumber)
    {
        return $"{IsFirstClearKey}_{stageNumber}";
    }
}
