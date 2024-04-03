using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime;

public class Stone : MonoBehaviour
{
    [SerializeField] private GameObject stoneObject = null;

    public Cell.Color color = Cell.Color.white;
    

    // Start is called before the first frame update
    async UniTask Start()
    {
        stoneObject.SetActive(false);

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
    async public UniTask Generate(float time) // default => 0.4f
    {
        await UniTask.Yield();

        Vector3 originScale = transform.localScale;

        this.transform.localScale = Vector3.zero;

        stoneObject.SetActive(true);

        await this.transform.DOScale(originScale, time).AsyncWaitForCompletion();

        await this.transform.DOMoveY(0, time).AsyncWaitForCompletion();

        // 【効果音再生箇所】生成時
        AudioManager.instance_AudioManager.PlaySE(2);
    }

    // 破棄時アニメーション
    async public UniTask Destroy(float time) // default => 0.4f
    {
        await UniTask.Yield();

        // 【効果音再生箇所】破棄時

        await this.transform.DOScale(Vector3.zero, time).AsyncWaitForCompletion();
    }

    // 裏返しアニメーション
    async public UniTask Flip()
    {
        await UniTask.Yield();

        Sequence sequence = DOTween.Sequence();

        //float rotation = (color == Cell.Color.black) ? 180.0f : 0f;

        //Debug.Log("【Stone】Flip() : 裏返し");

        await sequence.Append(this.transform.DOMoveY(1.0f, 0.4f))
                      .Join(transform.DORotate(new Vector3(0, 0, 180), 0.4f, RotateMode.WorldAxisAdd)).AsyncWaitForCompletion();

        await this.transform.DOMoveY(0, 0.4f).AsyncWaitForCompletion();

        // 【効果音再生箇所】パチッ！
        AudioManager.instance_AudioManager.PlaySE(2);
    }
}
