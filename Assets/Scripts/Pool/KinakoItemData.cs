public class KinakoItemData // MonoBehaviourがないからSceneViewのGameObjectに貼り付けれない
{
    public int id;
    public string name;
    public string text;

    // コンストラクタ => クラスをインスタンス化するときに呼ぶ（初期設定）
    public KinakoItemData(int id, string name, string text) 
    {
        this.id = id;
        this.name = name;
        this.text = text;
    }
}

