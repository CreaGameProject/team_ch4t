using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataManager : SingletonMonoBehaviour<SceneDataManager>
{
    private static string _prevSceneName; // 前のシーンの名前
    public string PrevSceneName => _prevSceneName;
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        LoadScene(name);
    }

    public void LoadScene(string name)
    {
        _prevSceneName = name;
        SceneManager.LoadScene(name);
    }
}
