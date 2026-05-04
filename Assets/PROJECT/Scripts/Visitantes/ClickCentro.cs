using UnityEngine;

public class ClickCentro : MonoBehaviour
{
    public float distanciaMax = 5f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click izquierdo
        {
            // Rayo desde el centro de la pantalla (puntero)
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distanciaMax))
            {
                Debug.Log($"Raycast golpeó: {hit.collider.name}");

                // Buscar botón VERDE (subiendo en la jerarquía)
                Transform actual = hit.collider.transform;
                bool encontradoVerde = false;
                while (actual != null && !encontradoVerde)
                {
                    if (actual.CompareTag("BotonVerde"))
                    {
                        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
                        if (visitante != null && visitante.enCentro && visitante.gameObject.activeInHierarchy)
                        {
                            visitante.Aceptar();
                            Debug.Log("✅ Click en VERDE - Visitante aceptado");
                            encontradoVerde = true;
                        }
                        else
                        {
                            Debug.Log("No hay visitante en el centro para aceptar");
                        }
                        break;
                    }
                    actual = actual.parent;
                }

                // Buscar botón ROJO (si no se encontró el verde)
                if (!encontradoVerde)
                {
                    actual = hit.collider.transform;
                    while (actual != null)
                    {
                        if (actual.CompareTag("BotonRojo"))
                        {
                            VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
                            if (visitante != null && visitante.enCentro && visitante.gameObject.activeInHierarchy)
                            {
                                visitante.Rechazar();
                                Debug.Log("❌ Click en ROJO - Visitante rechazado");
                            }
                            else
                            {
                                Debug.Log("No hay visitante en el centro para rechazar");
                            }
                            break;
                        }
                        actual = actual.parent;
                    }
                }
            }
        }
    }
}