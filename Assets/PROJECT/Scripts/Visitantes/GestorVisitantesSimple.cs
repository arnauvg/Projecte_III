using UnityEngine;
using System.Collections;

public class GestorVisitantesSimple : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject prefabVisitante;

    [Header("Puntos de movimiento")]
    public Transform puntoEntrada;
    public Transform puntoCentro;
    public Transform puntoEntradaEdificio;

    [Header("Tiempo")]
    public float tiempoEntreVisitantes = 3f;

    private VisitanteSimple visitanteActual;
    private bool esperandoVisitante = false;

    void Start()
    {
        CrearVisitanteActual();
    }

    void CrearVisitanteActual()
    {
        if (EstadoVisitantes.Instancia == null)
        {
            Debug.LogError("No existe EstadoVisitantes en la escena.");
            return;
        }

        VisitanteDatos datos = EstadoVisitantes.Instancia.ObtenerVisitanteActual();

        if (datos == null)
        {
            Debug.Log("No quedan visitantes. Aquí puedes terminar la noche.");

            GestionNoches gestionNoches = FindFirstObjectByType<GestionNoches>();

            if (gestionNoches != null)
            {
                gestionNoches.TerminarNoche();
            }

            return;
        }

        GameObject nuevoVisitante = Instantiate(prefabVisitante);
        visitanteActual = nuevoVisitante.GetComponent<VisitanteSimple>();

        if (visitanteActual == null)
        {
            Debug.LogError("El prefab no tiene el script VisitanteSimple.");
            return;
        }

        visitanteActual.ConfigurarVisitante(
            datos,
            puntoEntrada,
            puntoCentro,
            puntoEntradaEdificio,
            this
        );
    }

    public VisitanteSimple ObtenerVisitanteActual()
    {
        return visitanteActual;
    }

    public void VisitanteTerminoSalir()
    {
        if (esperandoVisitante) return;

        esperandoVisitante = true;

        EstadoVisitantes.Instancia.PasarAlSiguienteVisitante();

        StartCoroutine(EsperarYCrearSiguiente());
    }

    IEnumerator EsperarYCrearSiguiente()
    {
        yield return new WaitForSeconds(tiempoEntreVisitantes);

        esperandoVisitante = false;
        CrearVisitanteActual();
    }
    public void RegistrarRespuestaVisitante(bool aceptado)
    {
        if (visitanteActual == null) return;

        if (aceptado)
        {
            visitanteActual.Aceptar();
        }
        else
        {
            visitanteActual.Rechazar();
        }
    }
    public void RegistrarRespuestaVisitante()
    {
        Debug.Log("RegistrarRespuestaVisitante() llamado sin parámetro. No se hace nada porque ahora decide el botón verde o rojo.");
    }
}