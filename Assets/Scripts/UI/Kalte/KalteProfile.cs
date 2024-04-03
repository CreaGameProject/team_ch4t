using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="KalteProfileScriptable")]
public class KalteProfile : ScriptableObject
{
    [System.Serializable]
    public class Profile
    {
        public string name;
        public Sprite kalteImage;
        public string birthday;
        public string gradeAndClass;
        public int height;
        public int bodyWeight;
        public string hobby;
        public string specialSkill;
        [TextArea(3, 5)] public string[] secret = new string[3];
    }

    public List<Profile> profiles = new List<Profile>();
}
