using System;
using UniRx;
using UnityEngine;

namespace Pool
{
    public class ParticleObject : MonoBehaviour
    {
        private ParticleSystem _particle;

        private void Awake()
        {
            _particle = GetComponent<ParticleSystem>();
        }

        public IObservable<Unit> PlayParticle(Vector3 position)
        {
            transform.position = position;
            _particle.Play();

            // ParticleSystemのstartLifetimeに設定した秒数が経ったら終了通知
            return Observable.Timer(TimeSpan.FromSeconds(_particle.main.startLifetimeMultiplier))
                .ForEachAsync(_ => _particle.Stop());
        }
    }
}

