using System.Collections;
using System.Collections.Generic;
using Pool;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition - Camera.main.transform.forward * 5);

        if (Input.GetMouseButtonDown(0))
        {
            ParticleManager.Instance.GenerateParticle(ParticleType.Default, mousePos);
        }
    }
}
