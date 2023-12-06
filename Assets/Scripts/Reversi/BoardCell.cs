using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell : MonoBehaviour
{
    [SerializeField] private GameObject cellObject = null;

    private Vector3 cellObjectOriginScale = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        this.cellObjectOriginScale = this.cellObject.transform.localScale;
    }

    public async UniTask ShowCell(float animationTime)
    {
        this.cellObject.SetActive(true);

        // 回転アニメーション入れてみる？

        await this.cellObject.transform.DOScale(this.cellObjectOriginScale, animationTime).AsyncWaitForCompletion();
    }

    public async UniTask HideCell(float animationTime)
    {
        // 回転アニメーション入れてみる？

        await this.cellObject.transform.DOScale(Vector3.zero, animationTime).AsyncWaitForCompletion();

        this.cellObject.SetActive(false);
    }
}
