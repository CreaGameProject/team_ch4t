using System.Collections.Generic;
using Pool;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticlePoolData", menuName = "ScriptableObjects/CreateParticlePoolDataAsset")]
public class ParticlePoolDataAsset : ScriptableObject
{
    public List<ParticlePoolData> particlePoolDataList = new List<ParticlePoolData>();
}
