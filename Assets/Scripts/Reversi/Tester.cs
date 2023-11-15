using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // �Z���t�\���X�N���v�g�ɂ����ǉ�����
        // �f���Q�[�g�ɃC�x���g�n���h����ǉ�
        Board.instance.OnMethodAExecuted += OnMethodAExecutedHandler;
    }

    // �Z���t�\���X�N���v�g�ɂ����ǉ�����
    private void OnMethodAExecutedHandler()
    {
        // �L�����N�^�[�����鏈�����L�q����
        Debug.Log("�yTester�zOnMethodAExecutedHandler() | SpeakComputer()�����s����܂����I");
    }
}
