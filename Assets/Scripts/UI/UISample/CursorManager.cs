using Pool;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Camera overlayParticleCamera;
    
    private void Update()
    {
        Vector3 mousePos = overlayParticleCamera.ScreenToWorldPoint(Input.mousePosition + overlayParticleCamera.transform.forward * 5);

        if (Input.GetMouseButtonDown(0))
        {
            ParticleManager.Instance.GenerateParticle(ParticleType.Default, mousePos);
        }
    }
}
