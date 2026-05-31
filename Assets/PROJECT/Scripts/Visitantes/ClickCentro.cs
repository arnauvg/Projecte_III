using UnityEngine;

public class ClickCentro : MonoBehaviour
{
    public float distanciaMax = 5f;

    private GestorVisitantesSimple gestorVisitantes;
    private GameObject objetoApuntado;
    private Outline outlineApuntado;

    void Start()
    {
        gestorVisitantes = FindFirstObjectByType<GestorVisitantesSimple>();
    }

    void Update()
    {
        // Obtener el visitante actual
        VisitanteSimple visitante = null;
        if (gestorVisitantes != null)
        {
            visitante = gestorVisitantes.ObtenerVisitanteActual();
        }

        // ========== HOVER: SOLO SI EL VISITANTE ESTÁ EN EL CENTRO ==========
        if (visitante != null && visitante.enCentro)
        {
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
        }
        else
        {
            // Si el visitante NO está en el centro, NO mostrar outlines
            DesactivarOutline();
        }

        // ========== CLICK: SOLO SI EL VISITANTE ESTÁ EN EL CENTRO ==========
        if (Input.GetMouseButtonDown(0))
        {
            // Solo permitir interacción si el visitante está en el centro
            if (visitante == null || !visitante.enCentro) return;

            Ray rayClick = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(rayClick, out RaycastHit hit, distanciaMax))
            {
                // Buscar objeto revelador
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
                        // Activar animación de presión
                        BotonPresionAnimacion botonAnim = actual.GetComponent<BotonPresionAnimacion>();
                        if (botonAnim != null) botonAnim.Presionar();

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
                        // Activar animación de presión
                        BotonPresionAnimacion botonAnim = actual.GetComponent<BotonPresionAnimacion>();
                        if (botonAnim != null) botonAnim.Presionar();

                        visitante.Rechazar();
                        return;
                    }
                    actual = actual.parent;
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