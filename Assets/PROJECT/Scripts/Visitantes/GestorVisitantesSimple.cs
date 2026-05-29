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
            Debug.Log("No quedan visitantes.");

            // ❌ ELIMINA O COMENTA ESTAS LÍNEAS:
            // GestionNoches gestionNoches = FindFirstObjectByType<GestionNoches>();
            // if (gestionNoches != null)
            // {
            //     gestionNoches.TerminarNoche();  // ← Esto ya no existe
            // }

            // ✅ La noche ya NO termina aquí, solo por el reloj
            Debug.Log("Esperando a que termine la noche por el reloj (06:00 AM)");
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

    public void ReiniciarNoche()
    {
        // Limpiar visitante actual si existe
        if (visitanteActual != null)
            Destroy(visitanteActual.gameObject);

        esperandoVisitante = false;

        // Reiniciar el índice de visitantes
        if (EstadoVisitantes.Instancia != null)
            EstadoVisitantes.Instancia.indiceVisitanteActual = 0;

        CrearVisitanteActual();
    }

    public void ReiniciarJuegoCompleto()
    {
        ReiniciarNoche();
    }
}