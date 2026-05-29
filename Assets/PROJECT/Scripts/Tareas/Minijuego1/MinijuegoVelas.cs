using UnityEngine;
using TMPro;
using System.Collections;

public class MinijuegoVelas : MonoBehaviour
{
    public GameObject[] velasViejas;
    public GameObject[] velasNuevas;
    public TextMeshProUGUI textoEstado;
    public GameObject ZonaHuecosVelasNuevas;
    public CerrarMinijuego cerrarMinijuego; // Referencia al botón Salir

    private int velasViejasEliminadas = 0;
    private int velasNuevasColocadas = 0;
    private bool minijuegoCompletado = false;

    void Start()
    {
        ZonaHuecosVelasNuevas.SetActive(false);

        for (int i = 0; i < velasNuevas.Length; i++)
        {
            velasNuevas[i].SetActive(false);
        }

        if (textoEstado != null)
            textoEstado.text = "Retira las velas viejas y tíralas a la papelera";

        // Buscar CerrarMinijuego si no está asignado
        if (cerrarMinijuego == null)
            cerrarMinijuego = FindFirstObjectByType<CerrarMinijuego>();

        Debug.Log("MinijuegoVelas iniciado");
    }

    public void VelaViejaEliminada()
    {
        if (minijuegoCompletado) return;

        velasViejasEliminadas++;
        Debug.Log($"Vela vieja eliminada: {velasViejasEliminadas}/{velasViejas.Length}");

        if (velasViejasEliminadas >= velasViejas.Length)
        {
            ActivarVelasNuevas();
            ZonaHuecosVelasNuevas.SetActive(true);
        }
    }

    void ActivarVelasNuevas()
    {
        for (int i = 0; i < velasNuevas.Length; i++)
        {
            velasNuevas[i].SetActive(true);
        }

        if (textoEstado != null)
            textoEstado.text = "Ahora coloca las velas nuevas en su sitio";
    }

    public void VelaNuevaColocada()
    {
        if (minijuegoCompletado) return;

        velasNuevasColocadas++;
        Debug.Log($"Vela nueva colocada: {velasNuevasColocadas}/{velasNuevas.Length}");

        if (velasNuevasColocadas >= velasNuevas.Length && !minijuegoCompletado)
        {
            CompletarMinijuego();
        }
    }

    void CompletarMinijuego()
    {
        if (minijuegoCompletado) return;

        minijuegoCompletado = true;

        if (textoEstado != null)
            textoEstado.text = "¡Minijuego completado!";

        Debug.Log("Minijuego de velas completado");

        // NOTIFICAR AL SISTEMA DE NOCHES
        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
        {
            gestion.CompletarTarea();
            Debug.Log("Tarea de velas registrada");
        }

        // CERRAR CON DELAY Y SONIDO
        if (cerrarMinijuego != null)
        {
            StartCoroutine(CerrarConDelay());
        }
        else
        {
            Debug.LogWarning("cerrarMinijuego es NULL");
        }
    }

    IEnumerator CerrarConDelay()
    {
        Debug.Log("Esperando 0.8 segundos antes de cerrar...");
        yield return new WaitForSecondsRealtime(0.8f);

        cerrarMinijuego.CompletarYCerrar();
    }

    public void ReiniciarMinijuego()
    {
        velasViejasEliminadas = 0;
        velasNuevasColocadas = 0;
        minijuegoCompletado = false;

        ZonaHuecosVelasNuevas.SetActive(false);

        for (int i = 0; i < velasViejas.Length; i++)
        {
            velasViejas[i].SetActive(true);
        }

        for (int i = 0; i < velasNuevas.Length; i++)
        {
            velasNuevas[i].SetActive(false);
        }

        if (textoEstado != null)
            textoEstado.text = "Retira las velas viejas y tíralas a la papelera";
    }
}