using UnityEngine;

public class ClickCentro : MonoBehaviour
{
    public float distanciaMax = 5f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distanciaMax))
            {
                // Buscar botón VERDE
                Transform actual = hit.collider.transform;
                while (actual != null)
                {
                    if (actual.CompareTag("BotonVerde"))
                    {
                        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
                        if (visitante != null && visitante.enCentro)
                            visitante.Aceptar();
                        return;
                    }
                    actual = actual.parent;
                }

                // Buscar botón ROJO
                actual = hit.collider.transform;
                while (actual != null)
                {
                    if (actual.CompareTag("BotonRojo"))
                    {
                        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
                        if (visitante != null && visitante.enCentro)
                            visitante.Rechazar();
                        return;
                    }
                    actual = actual.parent;
                }
            }
        }
    }
}