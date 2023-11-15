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

    // 生成時アニメーション
    async public UniTask Generate()
    {
        await UniTask.Yield();

        Vector3 originScale = transform.localScale;

        this.transform.localScale = Vector3.zero;

        // 【効果音再生箇所】生成時

        await this.transform.DOScale(originScale, 0.4f).AsyncWaitForCompletion();
    }

    // 破棄時アニメーション
    async public UniTask Destroy()
    {
        await UniTask.Yield();

        // 【効果音再生箇所】破棄時

        await this.transform.DOScale(Vector3.zero, 0.4f).AsyncWaitForCompletion();
    }

    // 裏返しアニメーション
    async public UniTask Flip()
    {
        await UniTask.Yield();

        Sequence sequence = DOTween.Sequence();

        //float rotation = (color == Cell.Color.black) ? 180.0f : 0f;

        await sequence.Append(this.transform.DOMoveY(1.0f, 0.4f))
                      .Join(transform.DORotate(new Vector3(0, 0, 180), 0.4f, RotateMode.WorldAxisAdd)).AsyncWaitForCompletion();

        await this.transform.DOMoveY(0, 0.4f).AsyncWaitForCompletion();

        // 【効果音再生箇所】パチッ！
    }
}
