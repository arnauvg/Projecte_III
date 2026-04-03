using UnityEngine;

public class ClickCentro : MonoBehaviour
{
    public float distanciaMax = 5f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click izquierdo
        {
            Ray ray = Camera.main.ScreenPointToRay(
                new Vector3(Screen.width / 2, Screen.height / 2)
            );

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distanciaMax))
            {
                // VERDE
                if (hit.collider.CompareTag("BotonVerde"))
                {
                    FindObjectOfType<VisitanteSimple>().Aceptar();
                }

                // ROJO
                if (hit.collider.CompareTag("BotonRojo"))
                {
                    FindObjectOfType<VisitanteSimple>().Rechazar();
                }
            }
        }
    }
}