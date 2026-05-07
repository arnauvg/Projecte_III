using UnityEngine;

public class GestorVisitantesSimple : MonoBehaviour
{
    private GestionNoches gestionNoches;
    private bool visitanteAtendido = false;

    void Start()
    {
        gestionNoches = FindFirstObjectByType<GestionNoches>();

        if (gestionNoches == null)
            Debug.LogError("No se encontró GestionNoches!");
    }

    public void RegistrarRespuestaVisitante()
    {
        if (visitanteAtendido) return;

        visitanteAtendido = true;
        Debug.Log("Visitante atendido - Esperando que termine de salir");
    }

    public void VisitanteTerminoSalir()
    {
        Debug.Log("Visitante terminó de salir - Terminando noche");

        if (gestionNoches != null)
            gestionNoches.TerminarNoche();
    }
}