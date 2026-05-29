using UnityEngine;

public class ClickCentro : MonoBehaviour
{
    public float distanciaMax = 5f;

    private GestorVisitantesSimple gestorVisitantes;

    void Start()
    {
        gestorVisitantes = FindFirstObjectByType<GestorVisitantesSimple>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distanciaMax))
            {
                VisitanteSimple visitante = null;

                if (gestorVisitantes != null)
                {
                    visitante = gestorVisitantes.ObtenerVisitanteActual();
                }

                if (visitante == null) return;

                // Buscar objeto revelador, por ejemplo el ajo
                Transform objetoActual = hit.collider.transform;

                while (objetoActual != null)
                {
                    if (objetoActual.CompareTag("ObjetoRevelador"))
                    {
                        visitante.RevelarCamuflado();
                        return;
                    }

                    objetoActual = objetoActual.parent;
                }

                // Buscar botón VERDE
                Transform actual = hit.collider.transform;

                while (actual != null)
                {
                    if (actual.CompareTag("BotonVerde"))
                    {
                        if (visitante.enCentro)
                        {
                            visitante.Aceptar();
                        }

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
                        if (visitante.enCentro)
                        {
                            visitante.Rechazar();
                        }

                        return;
                    }

                    actual = actual.parent;
                }
            }
        }
    }
}