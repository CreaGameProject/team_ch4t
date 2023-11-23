using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime;

public class Stone : MonoBehaviour
{
    public Cell.Color color = Cell.Color.white;

    // Start is called before the first frame update
    async UniTask Start()
    {
        if (this.color == Cell.Color.black)
        {
            this.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
        }

        //await Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �������A�j���[�V����
    async public UniTask Generate()
    {
        await UniTask.Yield();

        Vector3 originScale = transform.localScale;

        this.transform.localScale = Vector3.zero;

        // �y���ʉ��Đ��ӏ��z������

        await this.transform.DOScale(originScale, 0.4f).AsyncWaitForCompletion();
    }

    // �j�����A�j���[�V����
    async public UniTask Destroy()
    {
        await UniTask.Yield();

        // �y���ʉ��Đ��ӏ��z�j����

        await this.transform.DOScale(Vector3.zero, 0.4f).AsyncWaitForCompletion();
    }

    // ���Ԃ��A�j���[�V����
    async public UniTask Flip()
    {
        await UniTask.Yield();

        Sequence sequence = DOTween.Sequence();

        //float rotation = (color == Cell.Color.black) ? 180.0f : 0f;

        Debug.Log("�yStone�zFlip() : ���Ԃ�");

        await sequence.Append(this.transform.DOMoveY(1.0f, 0.4f))
                      .Join(transform.DORotate(new Vector3(0, 0, 180), 0.4f, RotateMode.WorldAxisAdd)).AsyncWaitForCompletion();

        await this.transform.DOMoveY(0, 0.4f).AsyncWaitForCompletion();

        // �y���ʉ��Đ��ӏ��z�p�`�b�I
    }
}
