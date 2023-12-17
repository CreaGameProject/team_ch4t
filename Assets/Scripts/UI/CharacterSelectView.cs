using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectView : MonoBehaviour
{
    ScrollRect scrollRect;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.horizontalNormalizedPosition = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
