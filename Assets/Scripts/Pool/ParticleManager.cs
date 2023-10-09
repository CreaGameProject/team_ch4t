namespace Pool
{
    public class ParticleManager : SingletonMonoBehaviour<ParticleManager>
    {
        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(this);
        }
    }
}