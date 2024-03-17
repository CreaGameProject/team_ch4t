using UniRx.Toolkit;
using UnityEngine;

namespace Pool
{
    public class ParticlePool : ObjectPool<ParticleObject>
    {
        private readonly ParticleObject _prefab;
        private readonly Transform _parentTransform;

        public ParticlePool(Transform transform, ParticleObject prefab)
        {
            _parentTransform = transform;
            _prefab = prefab;
        }

        protected override ParticleObject CreateInstance()
        {
            var particleObject = Object.Instantiate(_prefab, _parentTransform, true);

            return particleObject;
        }
    }
}