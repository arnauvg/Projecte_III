using UnityEngine;

public class BotonEntrada : MonoBehaviour
{
    public VisitanteSimple visitante;

    void OnMouseDown()
    {
        if (visitante == null) return;
        visitante.Aceptar();
    }
}