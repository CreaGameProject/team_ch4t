using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Pool
{
    [RequireComponent(typeof(ParticlePoolDataHolder))]
    public class ParticleManager : SingletonMonoBehaviour<ParticleManager>
    {
        [SerializeField] private ParticlePoolDataHolder _poolData;
        private Dictionary<ParticleType, ParticlePool> _particleDictionary = new Dictionary<ParticleType, ParticlePool>();

        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
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

            //ObjectPoolから1つ取得
            var effect = pool.Rent();

            //エフェクトを再生し、再生終了したらpoolに返却する
            effect.PlayParticle(position).Subscribe(__ => { pool.Return(effect); });
        }
    }
}