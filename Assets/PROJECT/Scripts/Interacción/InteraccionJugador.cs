using UnityEngine;

public class InteraccionJugador : MonoBehaviour
{
    public float distanciaInteraccion = 5f;
    private GameObject objetoApuntado;
    private Outline outlineApuntado;
    private Interactuable objetoEnMano;

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (objetoEnMano == null)
        {
            // Obtener TODOS los objetos que el rayo golpea
            RaycastHit[] hits = Physics.RaycastAll(ray, distanciaInteraccion);

            GameObject mejorObjeto = null;

            // Primero: Buscar un objeto Interactuable (como la biblia)
            foreach (RaycastHit hit in hits)
            {
                GameObject objetoActual = hit.collider.gameObject;

                // Buscar componente Interactuable en el objeto o en su padre
                Interactuable interactuable = objetoActual.GetComponent<Interactuable>();
                if (interactuable == null && objetoActual.transform.parent != null)
                    interactuable = objetoActual.transform.parent.GetComponent<Interactuable>();

                // Si encontramos un Interactuable, verificamos si es válido
                if (interactuable != null)
                {
                    // Verificar si es un objeto recogible y está dentro de un cajón cerrado
                    Recogible recogible = interactuable as Recogible;
                    if (recogible != null)
                    {
                        // Si es recogible y está en un cajón cerrado, lo ignoramos
                        if (recogible.EstaEnCajonCerrado())
                        {
                            continue; // Saltar este objeto, no seleccionarlo
                        }
                    }

                    mejorObjeto = interactuable.gameObject;
                    break; // Salir del bucle, prioridad máxima
                }
            }

            // Segundo: Si no hay Interactuable, buscar un Cajón
            if (mejorObjeto == null)
            {
                foreach (RaycastHit hit in hits)
                {
                    GameObject objetoActual = hit.collider.gameObject;
                    Cajon cajon = objetoActual.GetComponent<Cajon>();

                    if (cajon != null)
                    {
                        mejorObjeto = cajon.gameObject;
                        break;
                    }
                }
            }

            // Activar o desactivar outline según corresponda
            if (mejorObjeto != null && mejorObjeto != objetoApuntado)
            {
                DesactivarOutline();
                objetoApuntado = mejorObjeto;
                ActivarOutline();
            }
            else if (mejorObjeto == null)
            {
                DesactivarOutline();
            }
        }
        else
        {
            DesactivarOutline();
        }

        // ========== CLICK DEL RATÓN ==========
        if (Input.GetMouseButtonDown(0))
        {
            // Si tenemos algo en la mano, lo soltamos
            if (objetoEnMano != null)
            {
                objetoEnMano.Soltar();
                objetoEnMano = null;
            }
            else
            {
                // Obtener TODOS los objetos que el rayo golpea
                RaycastHit[] hits = Physics.RaycastAll(ray, distanciaInteraccion);

                // Primero: Buscar Interactuable (biblia, teléfono, etc.)
                foreach (RaycastHit hit in hits)
                {
                    GameObject objetoActual = hit.collider.gameObject;

                    // Buscar Interactuable en el objeto o su padre
                    Interactuable interactuable = objetoActual.GetComponent<Interactuable>();
                    if (interactuable == null && objetoActual.transform.parent != null)
                        interactuable = objetoActual.transform.parent.GetComponent<Interactuable>();

                    if (interactuable != null)
                    {
                        // Verificar si es un objeto recogible y está en un cajón cerrado
                        Recogible recogible = interactuable as Recogible;
                        if (recogible != null)
                        {
                            if (recogible.EstaEnCajonCerrado())
                            {
                                continue; // No permitir recoger de un cajón cerrado
                            }
                        }

                        Debug.Log($"Intentando recoger: {interactuable.gameObject.name}");
                        if (interactuable.Recoger())
                            objetoEnMano = interactuable;
                        return; // Salir después de recoger
                    }
                }

                // Segundo: Buscar Cajón
                foreach (RaycastHit hit in hits)
                {
                    GameObject objetoActual = hit.collider.gameObject;
                    Cajon cajon = objetoActual.GetComponent<Cajon>();

                    if (cajon != null)
                    {
                        Debug.Log($"Abriendo cajón: {cajon.gameObject.name}");
                        cajon.Interact();
                        return;
                    }
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