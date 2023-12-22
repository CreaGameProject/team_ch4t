using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CustomButton>().onClick.AddListener(PlayPressedButtonSe);

        if (GetComponent<EventTrigger>() == null) { gameObject.AddComponent<EventTrigger>(); }

        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventDate) => { PlayHighlightedButtonSe(); });
        trigger.triggers.Add(entry);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPressedButtonSe()
    {
        AudioManager.instance_AudioManager.PlaySE(0);
    }

    public void PlayHighlightedButtonSe()
    {
        AudioManager.instance_AudioManager.PlaySE(1);    
    }
}
