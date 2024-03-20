using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    public delegate void OverlayOpenedEventHandler(bool isOpen);
    public static event OverlayOpenedEventHandler OverlayOpened;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ウィンドウやカルテの開閉時に呼ばれる
    /// </summary>
    /// <param name="isOpen">開いているとtrue</param>
    public static void OnOverlayOpened(bool isOpen)
    {
        OverlayOpened(isOpen);
    }
}
