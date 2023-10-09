using UnityEngine;

namespace UI.UISample
{
    public class UICustomButtonSample : MonoBehaviour
    {
        [SerializeField] private CustomButton _button;
        private void Start()
        {
            _button.OnClickAction = () =>
            {
                Debug.Log("Click");
            };
            
            _button.OnLongPressAction = () =>
            {
                Debug.Log("LongPress");
            };
        }
    }
}