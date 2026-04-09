using UnityEngine;

public class InteraccionManual : MonoBehaviour
{
    public float distancia = 3f; // distancia máxima para interactuar

    void Update()
    {
        // Ray desde el centro de la pantalla
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distancia))
        {
            // Click izquierdo
            if (Input.GetMouseButtonDown(0))
            {
                // Buscar el script del manual
                ManualInteractuable manual = hit.collider.GetComponentInParent<ManualInteractuable>();

                if (manual != null)
                {
                    manual.Interactuar();
                }
            }
        }
    }
}