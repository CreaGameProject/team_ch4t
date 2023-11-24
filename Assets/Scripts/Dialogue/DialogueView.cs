using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    public TextMeshProUGUI NameText => nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    public TextMeshProUGUI DialogueText => dialogueText;
    [SerializeField] private Image dialogueCharacterImage;
    public Image DialogueCharacterImage => dialogueCharacterImage;

}
