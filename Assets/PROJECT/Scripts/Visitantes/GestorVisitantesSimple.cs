using UnityEngine;

public class GestorVisitantes : MonoBehaviour
{
    [Header("Configuración")]
    public VisitanteSimple visitante;
    public float tiempoEntreVisitantes = 2f;

    private GestionNoches gestionNoches;
    private int visitantesAtendidos = 0;

    void Start()
    {
        gestionNoches = FindFirstObjectByType<GestionNoches>();

        if (visitante == null)
            visitante = FindFirstObjectByType<VisitanteSimple>();

        if (visitante == null || gestionNoches == null)
        {
            Debug.LogError("Faltan referencias!");
            return;
        }

        IniciarNoche();
    }

    public void IniciarNoche()
    {
        visitantesAtendidos = 0;
        visitante.ReiniciarParaNuevaNoche();
        Debug.Log("Noche iniciada");
    }

    public void RegistrarRespuestaVisitante()
    {
        visitantesAtendidos++;
        Debug.Log($"Visitante atendido ({visitantesAtendidos}/3)");

        if (visitantesAtendidos >= 3)
        {
            if (gestionNoches != null)
                gestionNoches.TerminarNoche();
        }
        else
        {
            Invoke(nameof(SiguienteVisitante), tiempoEntreVisitantes);
        }
    }

    void SiguienteVisitante()
    {
        visitante.ReiniciarParaNuevaNoche();
        Debug.Log("Siguiente visitante aparece");
    }
}