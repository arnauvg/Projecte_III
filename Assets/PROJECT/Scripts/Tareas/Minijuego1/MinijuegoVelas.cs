using UnityEngine;
using TMPro;
using System.Collections;

public class MinijuegoVelas : MonoBehaviour
{
    public GameObject[] velasViejas;
    public GameObject[] velasNuevas;
    public TextMeshProUGUI textoEstado;
    public GameObject ZonaHuecosVelasNuevas;
    public CerrarMinijuego cerrarMinijuego;   // ← arrastra el botón Salir

    private int velasViejasEliminadas = 0;
    private int velasNuevasColocadas = 0;
    private bool completado = false;

    void Start()
    {
        ZonaHuecosVelasNuevas.SetActive(false);
        foreach (var v in velasNuevas) v.SetActive(false);
        if (cerrarMinijuego == null) cerrarMinijuego = FindObjectOfType<CerrarMinijuego>();
    }

    public void VelaViejaEliminada()
    {
        if (completado) return;
        velasViejasEliminadas++;
        if (velasViejasEliminadas >= velasViejas.Length)
        {
            ActivarVelasNuevas();
            ZonaHuecosVelasNuevas.SetActive(true);
        }
    }

    void ActivarVelasNuevas()
    {
        foreach (var v in velasNuevas) v.SetActive(true);
        if (textoEstado != null) textoEstado.text = "Coloca las velas nuevas";
    }

    public void VelaNuevaColocada()
    {
        if (completado) return;
        velasNuevasColocadas++;
        if (velasNuevasColocadas >= velasNuevas.Length && !completado)
        {
            CompletarMinijuego();
        }
    }

    void CompletarMinijuego()
    {
        completado = true;
        if (textoEstado != null) textoEstado.text = "¡Completado!";

        GestionNoches gestion = FindObjectOfType<GestionNoches>();
        if (gestion != null) gestion.CompletarTarea();

        if (cerrarMinijuego != null)
            StartCoroutine(CerrarConDelay());
    }

    IEnumerator CerrarConDelay()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        cerrarMinijuego.CompletarYCerrar();
    }
}