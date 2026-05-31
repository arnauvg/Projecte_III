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
        // ========== HOVER: DETECTAR OUTLINE ==========
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

        // ========== CLICK ==========
        if (Input.GetMouseButtonDown(0))
        {
            Ray rayClick = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(rayClick, out hit, distanciaMax))
            {
                VisitanteSimple visitante = null;

                if (gestorVisitantes != null)
                {
                    visitante = gestorVisitantes.ObtenerVisitanteActual();
                }

                if (visitante == null) return;

                // Buscar objeto revelador (ej: ajo, cruz, etc.)
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
                        // 👇 ANIMACIÓN DE PRESIÓN
                        BotonPresionAnimacion botonAnim = actual.GetComponent<BotonPresionAnimacion>();
                        if (botonAnim != null) botonAnim.Presionar();

                        if (visitante.enCentro) visitante.Aceptar();
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
                        // 👇 ANIMACIÓN DE PRESIÓN
                        BotonPresionAnimacion botonAnim = actual.GetComponent<BotonPresionAnimacion>();
                        if (botonAnim != null) botonAnim.Presionar();

                        if (visitante.enCentro) visitante.Rechazar();
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