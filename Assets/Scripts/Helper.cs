using System.Collections.Generic;
using UnityEngine;

public class Helper
{
    internal static T GetRandom<T> (IList<T> Params)
    {
        return Params [Random.Range (0, Params.Count)];
    }

    public const string CharacterFilePath = "Sprites/CAS_character_portraits_for_dialogs_vol1/";
}