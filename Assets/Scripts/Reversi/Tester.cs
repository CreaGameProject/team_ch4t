using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Board;

public class Tester : MonoBehaviour
{
    private void Start()
    {
        // �萔���擾����
        // �� 0 �ɂȂ�����Q�[���I�[�o�[
        int restTurn = Board.instance.getPresetRestTurn;
        Debug.Log(string.Format("�yTester�zrestTurn | {0}", restTurn));

        // ��Łi���ݒN�̃^�[�����j���擾����
        // �� turn => neutral : �ǂ���ł��Ȃ�
        // �� turn => player : �v���C���[�̃^�[��
        // �� turn => computer : �R���s���[�^�̃^�[��
        Turn.Type turn = Board.instance.getTurn;
        Debug.Log(string.Format("�yTester�zrestTurn | {0}", turn));

        // �ΐ푊����擾����i�ΐ푊��̖��O�j
        // �� opponent => None : �N�ł��Ȃ��i������ԁj
        // �� opponent => Yukihira_ui : �ᕽ �D�ˁi�䂫�Ђ� �����j
        // �� opponent => Takahashi_shota : ���� �đ��i�����͂� ���傤���j
        //Computer.Opponent opponent = Board.instance.getOpponent;
        Computer.Opponent opponent = Computer.opponent;
        Debug.Log(string.Format("�yTester�zopponent | {0}", opponent));

        // ���ݎ擾���Ă���q�~�c�̐����擾����
        int howManyHimituDidGet = Board.instance.getHowManyHimituDidGet;
        Debug.Log(string.Format("�yTester�zhowManyHimituDidGet | {0}", howManyHimituDidGet));

        Board.instance.OnChangeRestTurnExecuted += OnChangeRestTurnExecutedHandler;
        Board.instance.OnChangeTurnExecuted += OnChangeTurnExecutedHandler;
        Board.instance.OnChangeHimituNumberExecuted += OnChangeHimituNumberExecutedHandler;
        Board.instance.OnGameOverExecuted += OnGameOverExecutedHandler;

        // �Z���t�\���X�N���v�g�ɂ����ǉ�����
        Board.instance.OnSpeakComputerExecuted += OnSpeakComputerExecutedHandler;
        Board.instance.OnSecretCellPerformanceExecuted += OnSecretCellPerformanceExecutedHandler;

        // 
        Board.instance.ImpossiblePlaceStonesExecuted += ImpossiblePlaceStones;
    }

    // �c��萔���ύX���ꂽ�Ƃ��Ɏ��s�����
    // �����FrestTurn�F�ύX���ꂽ��̎c��̎萔
    private void OnChangeRestTurnExecutedHandler(int restTurn)
    {
        // �c��萔���ύX���ꂽ�Ƃ��̏������L�q����
        Debug.Log("�yTester�zOnChangeRestTurnExecutedHandler | �c��萔���ύX���ꂽ�Ƃ��̏������L�q����");
    }

    // ��ԁi���݂̃^�[���j���ύX���ꂽ�Ƃ��Ɏ��s�����
    // �����Ftype�F�ύX���ꂽ��̎�ԁi���݂̃^�[���j�F
    private void OnChangeTurnExecutedHandler(Turn.Type type)
    {
        // ��ԁi���݂̃^�[���j���ύX���ꂽ�Ƃ��̏������L�q����
        Debug.Log("�yTester�zOnChangeRestTurnExecutedHandler | ��ԁi���݂̃^�[���j���ύX���ꂽ�Ƃ��̏������L�q����");
    }

    // ���ݎ擾���Ă���q�~�c�̐����ς��������s�����
    // �����FhowManyHimituDidGet�F�ύX���ꂽ��̌��ݎ擾���Ă���q�~�c�̐�
    private void OnChangeHimituNumberExecutedHandler(int howManyHimituDidGet)
    {
        // ���ݎ擾���Ă���q�~�c�̐����ύX���ꂽ�Ƃ��̏������L�q����
        Debug.Log("�yTester�zOnChangeRestTurnExecutedHandler | ���ݎ擾���Ă���q�~�c�̐����ύX���ꂽ�Ƃ��̏������L�q����");
    }

    // �΋ǂ�������������s�����
    // �����FgameResult�F�΋ǂ̌���
    // �� gameResult => None : �������
    // �� gameResult => Player_WIN : �v���C���[�̏���
    // �� gameResult => Player_LOSE : �v���C���[�̕���
    // �� gameResult => Drow : ��������
    private void OnGameOverExecutedHandler(GameResult gameResult)
    {
        // �΋ǂ�������������s�����
        Debug.Log("�yTester�zOnChangeRestTurnExecutedHandler | �΋ǂ�������������s�����");

        Debug.Log(string.Format("�yTester�zOnChangeRestTurnExecutedHandler | gameResult : {0}", gameResult));
    }

    async public UniTask OnSecretCellPerformanceExecutedHandler()
    {
        // �q�~�c�}�X�����Ԃ��ꂽ�Ƃ��̉��o���L�q����
        Debug.Log("�yTester�zOnSecretCellPerformanceExecutedHandler | �q�~�c�}�X�����Ԃ��ꂽ�Ƃ��̉��o���L�q����");

        await UniTask.Yield();
    }

    async public UniTask OnSpeakComputerExecutedHandler()
    {
        // �L�����N�^�[�����鏈�����L�q����
        Debug.Log("�yTester�zOnSpeakComputerExecutedHandler() | �L�����N�^�[�����鏈�����L�q����");

        await UniTask.Yield();
    }


    public bool popUpWindowLock = false; // �u�͂��v�{�^�����������Ƃ��� true ��
    async public UniTask ImpossiblePlaceStones()
    {
        await UniTask.Yield();

        // �E�B���h�E���|�b�v�A�b�v�����鏈��

        while (!popUpWindowLock)
        {
            // 100�~���b�ҋ@���čĎ��s
            await UniTask.Delay(10);
        }

        popUpWindowLock = false;

        // 
    }
}
