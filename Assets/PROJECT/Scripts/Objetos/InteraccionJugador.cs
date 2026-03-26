using UnityEngine;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaInteraccion = 5f;

    private GameObject objetoApuntado;
    private Outline outlineApuntado;
    private Interactuable objetoEnMano; // Referencia al objeto que tenemos agarrado

    void Update()
    {
        // Lanzar rayo desde el centro de la pantalla para detectar objetos
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Detectar outline SOLO si no tenemos nada en la mano
        if (objetoEnMano == null)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
            {
                Interactuable interactuable = hit.collider.GetComponent<Interactuable>();

                if (interactuable != null)
                {
                    if (objetoApuntado != hit.collider.gameObject)
                    {
                        DesactivarOutline();
                        objetoApuntado = hit.collider.gameObject;
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
                DesactivarOutline();
            }
        }
        else
        {
            // Si tenemos algo en la mano, no mostramos outline de otros objetos
            DesactivarOutline();
        }

        // Detectar click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            if (objetoEnMano != null)
            {
                // Si tenemos algo en la mano, soltarlo según su tipo
                objetoEnMano.Soltar();
                objetoEnMano = null;
            }
            else
            {
                // Si no tenemos nada, intentar recoger
                if (Physics.Raycast(ray, out RaycastHit hitClick, distanciaInteraccion))
                {
                    Interactuable interactuable = hitClick.collider.GetComponent<Interactuable>();

                    if (interactuable != null)
                    {
                        // Intentar recoger el objeto
                        if (interactuable.Recoger())
                        {
                            objetoEnMano = interactuable;
                        }
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
            {
                outlineApuntado = objetoApuntado.AddComponent<Outline>();
            }

            outlineApuntado.OutlineMode = Outline.Mode.OutlineAll;
            outlineApuntado.OutlineColor = Color.yellow;
            outlineApuntado.OutlineWidth = 4f;
        }
    }

    void DesactivarOutline()
    {
        if (outlineApuntado != null)
        {
            Destroy(outlineApuntado);
            outlineApuntado = null;
        }
        objetoApuntado = null;
    }
}