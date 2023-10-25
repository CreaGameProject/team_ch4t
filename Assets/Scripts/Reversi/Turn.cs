using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour
{
    [System.Serializable]
    public enum Type
    {
        neutral = 0,
        player = 1,
        computer = 2,
    }
}
