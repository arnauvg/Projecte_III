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
            if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
            {
                Interactuable interactuable = hit.collider.GetComponent<Interactuable>();
                Cajon cajon = hit.collider.GetComponent<Cajon>();
                if (interactuable != null || cajon != null)
                {
                    GameObject nuevo = hit.collider.gameObject;
                    if (objetoApuntado != nuevo)
                    {
                        DesactivarOutline();
                        objetoApuntado = nuevo;
                        ActivarOutline();
                    }
                }
                else DesactivarOutline();
            }
            else DesactivarOutline();
        }
        else DesactivarOutline();

        if (Input.GetMouseButtonDown(0))
        {
            if (objetoEnMano != null)
            {
                objetoEnMano.Soltar();
                objetoEnMano = null;
            }
            else if (Physics.Raycast(ray, out RaycastHit hitClick, distanciaInteraccion))
            {
                Cajon cajon = hitClick.collider.GetComponent<Cajon>();
                if (cajon != null)
                {
                    cajon.Interact();
                    return;
                }
                Interactuable interactuable = hitClick.collider.GetComponent<Interactuable>();
                if (interactuable != null && interactuable.Recoger())
                    objetoEnMano = interactuable;
            }
        }
    }

    void ActivarOutline()
    {
        if (objetoApuntado != null)
        {
            outlineApuntado = objetoApuntado.GetComponent<Outline>();
            if (outlineApuntado == null) outlineApuntado = objetoApuntado.AddComponent<Outline>();
            outlineApuntado.OutlineMode = Outline.Mode.OutlineAll;
            outlineApuntado.OutlineColor = Color.yellow;
            outlineApuntado.OutlineWidth = 4f;
        }
    }
    void DesactivarOutline()
    {
        if (outlineApuntado != null) Destroy(outlineApuntado);
        objetoApuntado = null;
    }
}