using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class ParticlePoolDataHolder : MonoBehaviour

    {
        [SerializeField] private ParticlePoolDataAsset particlePoolDataAsset;
        public IReadOnlyList<ParticlePoolData> ParticlePoolDataList => particlePoolDataAsset.particlePoolDataList;
    }
}