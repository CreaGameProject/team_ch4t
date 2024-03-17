using UnityEngine;

namespace Pool
{
    [System.Serializable]
    public class ParticlePoolData
    {
        [SerializeField] private ParticleType particleType;
        public ParticleType ParticleType => particleType;
        [SerializeField] private ParticleObject particleObject;
        public ParticleObject ParticleObject => particleObject;
        [SerializeField] private int defaultPoolStock;
        public int DefaultPoolStock => defaultPoolStock;
    }
}