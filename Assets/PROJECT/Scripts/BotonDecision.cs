using UnityEngine;

public class BotonDecision : MonoBehaviour
{
    public bool esVerde; // true = entrar, false = sortir
    public VisitanteSimple visitante;

    public void Pulsar()
    {
        if (visitante == null) return;

        if (esVerde)
        {
            visitante.Aceptar();   // entra
        }
        else
        {
            visitante.Rechazar();  // surt
        }
    }
}