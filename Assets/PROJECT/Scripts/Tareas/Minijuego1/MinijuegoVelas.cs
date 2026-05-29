using UnityEngine;
using TMPro;

public class MinijuegoVelas : MonoBehaviour
{
    public GameObject[] velasViejas;
    public GameObject[] velasNuevas;
    public TextMeshProUGUI textoEstado;
    public GameObject ZonaHuecosVelasNuevas;
    public GameObject botonCerrar; // Botón de salir del minijuego

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

        // Si hay botón de cerrar, desactivarlo hasta completar
        if (botonCerrar != null)
            botonCerrar.SetActive(false);
    }

    public void VelaViejaEliminada()
    {
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
        velasNuevasColocadas++;
        Debug.Log($"Vela nueva colocada: {velasNuevasColocadas}/{velasNuevas.Length}");

        if (velasNuevasColocadas >= velasNuevas.Length && !minijuegoCompletado)
        {
            CompletarMinijuego();
        }
    }

    void CompletarMinijuego()
    {
        minijuegoCompletado = true;

        if (textoEstado != null)
            textoEstado.text = "¡Minijuego completado! Puedes salir";

        Debug.Log("Minijuego de velas completado");

        // NOTIFICAR AL SISTEMA DE NOCHES
        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
        {
            gestion.CompletarTarea();
            Debug.Log("Tarea de velas registrada en el sistema de noches");
        }

        // Activar botón de cerrar
        if (botonCerrar != null)
            botonCerrar.SetActive(true);
    }
}