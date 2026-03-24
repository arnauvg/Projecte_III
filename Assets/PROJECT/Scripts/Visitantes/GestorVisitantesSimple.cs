using UnityEngine;

public class GestorVisitantes : MonoBehaviour
{
    [Header("Referencias")]
    public VisitanteSimple visitante;

    public float tiempoPrimeraAparicion = 1f;

    void Start()
    {
        if (visitante == null)
        {
            Debug.LogError("No hay visitante asignado!");
            return;
        }

        Invoke(nameof(LlamarVisitante), tiempoPrimeraAparicion);
    }

    void LlamarVisitante()
    {
        Debug.Log("Llamando al visitante...");
        visitante.Aparecer();
    }
}