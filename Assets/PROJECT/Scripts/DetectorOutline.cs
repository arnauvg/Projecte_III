using UnityEngine;

public class DetectorOutline : MonoBehaviour
{
    public float distancia = 5f;
    public bool usarCentroPantalla = false;

    private ObjetoOutline objetoActual;

    void Update()
    {
        Ray ray;

        if (usarCentroPantalla)
        {
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        }
        else
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distancia))
        {
            ObjetoOutline objeto = hit.collider.GetComponentInParent<ObjetoOutline>();

            if (objeto != null)
            {
                if (objetoActual != objeto)
                {
                    QuitarOutline();

                    objetoActual = objeto;
                    objetoActual.ActivarOutline();
                }

                return;
            }
        }

        QuitarOutline();
    }

    void QuitarOutline()
    {
        if (objetoActual != null)
        {
            objetoActual.DesactivarOutline();
            objetoActual = null;
        }
    }
}