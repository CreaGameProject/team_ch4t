using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Board;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Board.instance != null)
        {
            Debug.Log("�ySceneChanger - Start�zBoard.instance != null");
            Board.instance.OnGameOverExecuted += OnGameOverExecutedHandler;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadCharacterSelectScene()
    {
        StageManager stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        if (stageManager.GetHasCleared(1))
        {
            // �X�e�[�W1���N���A���Ă�����A�L�����N�^�[�I����ʂ�
            LoadScene("CharacterSelect");
        }
        else
        {
            // �N���A���Ă��Ȃ�������A�L�����N�^�[�I����ʂ��X�L�b�v
            Computer.opponent = Computer.Opponent.Yukihira_ui;
            LoadScene("Dialogue");
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
        Application.Quit();//�Q�[���v���C�I��
#endif
    }

    // �����FgameResult�F�΋ǂ̌���
    // �� gameResult => None : �������
    // �� gameResult => Player_WIN : �v���C���[�̏���
    // �� gameResult => Player_LOSE : �v���C���[�̕���
    // �� gameResult => Drow : ��������
    private void OnGameOverExecutedHandler(GameResult gameResult)
    {
        // �΋ǂ�������������s�����
        //Debug.Log("�yTester�zOnChangeRestTurnExecutedHandler | �΋ǂ�������������s�����");
        //Debug.Log(string.Format("�yTester�zOnChangeRestTurnExecutedHandler | gameResult : {0}", gameResult));
        if (gameResult == GameResult.Player_WIN)
        {
            LoadScene("Clear");
        }
        else
        {
            LoadScene("Gameover");
        }
    }
}
