using UnityEngine;

public class ClickCentro : MonoBehaviour
{
    public float distanciaMax = 5f;
    private GameObject objetoApuntado;
    private Outline outlineApuntado;

    void Update()
    {
        // Detectar hover con Raycast
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaMax))
        {
            GameObject nuevoObjeto = null;
            Transform actual = hit.collider.transform;

            while (actual != null)
            {
                if (actual.CompareTag("BotonVerde") || actual.CompareTag("BotonRojo"))
                {
                    nuevoObjeto = actual.gameObject;
                    break;
                }
                actual = actual.parent;
            }

            if (nuevoObjeto != objetoApuntado)
            {
                DesactivarOutline();
                objetoApuntado = nuevoObjeto;
                if (objetoApuntado != null)
                    ActivarOutline();
            }
        }
        else
        {
            DesactivarOutline();
        }

        // Input click
        if (Input.GetMouseButtonDown(0))
        {
            Ray rayClick = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(rayClick, out hit, distanciaMax))
            {
                // Buscar objeto revelador
                Transform objetoActual = hit.collider.transform;
                while (objetoActual != null)
                {
                    if (objetoActual.CompareTag("ObjetoRevelador"))
                    {
                        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
                        if (visitante != null) visitante.RevelarCamuflado();
                        return;
                    }
                    objetoActual = objetoActual.parent;
                }

                // Buscar botón VERDE
                Transform actualVerde = hit.collider.transform;
                while (actualVerde != null)
                {
                    if (actualVerde.CompareTag("BotonVerde"))
                    {
                        Debug.Log($"Click en botón VERDE: {actualVerde.name}");

                        // Activar animación de presión
                        BotonPresionAnimacion botonAnim = actualVerde.GetComponent<BotonPresionAnimacion>();
                        if (botonAnim != null)
                        {
                            botonAnim.Presionar();
                        }
                        else
                        {
                            Debug.LogError($"No se encontró BotonPresionAnimacion en {actualVerde.name}");
                        }

                        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
                        if (visitante != null && visitante.enCentro) visitante.Aceptar();
                        return;
                    }
                    actualVerde = actualVerde.parent;
                }

                // Buscar botón ROJO
                Transform actualRojo = hit.collider.transform;
                while (actualRojo != null)
                {
                    if (actualRojo.CompareTag("BotonRojo"))
                    {
                        Debug.Log($"Click en botón ROJO: {actualRojo.name}");

                        // Activar animación de presión
                        BotonPresionAnimacion botonAnim = actualRojo.GetComponent<BotonPresionAnimacion>();
                        if (botonAnim != null)
                        {
                            botonAnim.Presionar();
                        }
                        else
                        {
                            Debug.LogError($"No se encontró BotonPresionAnimacion en {actualRojo.name}");
                        }

                        VisitanteSimple visitante = FindFirstObjectByType<VisitanteSimple>();
                        if (visitante != null && visitante.enCentro) visitante.Rechazar();
                        return;
                    }
                    actualRojo = actualRojo.parent;
                }
            }
        }
    }

    void ActivarOutline()
    {
        if (objetoApuntado != null)
        {
            outlineApuntado = objetoApuntado.GetComponent<Outline>();
            if (outlineApuntado == null)
                outlineApuntado = objetoApuntado.AddComponent<Outline>();

            outlineApuntado.OutlineMode = Outline.Mode.OutlineAll;
            outlineApuntado.OutlineColor = Color.yellow;
            outlineApuntado.OutlineWidth = 4f;
        }
    }

    void DesactivarOutline()
    {
        if (outlineApuntado != null)
            Destroy(outlineApuntado);
        objetoApuntado = null;
    }
}