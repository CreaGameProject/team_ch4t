using UnityEngine;
using TMPro;

public class LogNode : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI content;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetContent(string content)
    {
        this.content.text = content;
    }
}
