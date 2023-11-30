using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Pool
{
    [RequireComponent(typeof(ParticlePoolDataHolder))]
    public class ParticleManager : SingletonMonoBehaviour<ParticleManager>
    {
        private ParticlePoolDataHolder _poolData;
        private Dictionary<ParticleType, ParticlePool> _particleDictionary = new Dictionary<ParticleType, ParticlePool>();

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _poolData = GetComponent<ParticlePoolDataHolder>();
            foreach (var data in _poolData.ParticlePoolDataList)
            {
                var parentObject = new GameObject(data.ParticleType + "ParticlePool");
                var particlePool = new ParticlePool(parentObject.transform, data.ParticleObject);
                particlePool.PreloadAsync(10, 10).Subscribe();
                _particleDictionary.Add(data.ParticleType, particlePool);
            }
        }
        
        public void GenerateParticle(ParticleType particleType, Vector3 position)
        {
            var pool = _particleDictionary.GetValueOrDefault(particleType);

            if (pool == null)
            {
                return;
            }

            var effect = pool.Rent(); //ObjectPoolから1つ取得
            effect.PlayParticle(position).Subscribe(__ => { pool.Return(effect); }); //エフェクトを再生し、再生終了したらpoolに返却する
        }
    }
}