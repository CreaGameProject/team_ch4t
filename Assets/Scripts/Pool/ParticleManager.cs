using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    [RequireComponent(typeof(ParticlePoolDataHolder))]
    public class ParticleManager : SingletonMonoBehaviour<ParticleManager>
    {
        [SerializeField] private ParticlePoolDataHolder _poolData;
        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            foreach (var data in _poolData.ParticlePoolDataList)
            {
                Debug.Log($"Type: {data.ParticleType.ToString()}, Amount: {data.DefaultPoolStock}");
            }
        }
    }
}