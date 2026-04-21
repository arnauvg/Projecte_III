using UnityEngine;
using TMPro;

public class MinijuegoVelas : MonoBehaviour
{
    public GameObject[] velasViejas;
    public GameObject[] velasNuevas;
    public TextMeshProUGUI textoEstado;

    private int velasViejasEliminadas = 0;
    private int velasNuevasColocadas = 0;

    void Start()
    {
        // Al empezar, las velas nuevas están ocultas
        for (int i = 0; i < velasNuevas.Length; i++)
        {
            velasNuevas[i].SetActive(false);
        }

        if (textoEstado != null)
            textoEstado.text = "Retira las velas viejas y tíralas a la papelera";
    }

    public void VelaViejaEliminada()
    {
        velasViejasEliminadas++;

        if (velasViejasEliminadas >= velasViejas.Length)
        {
            ActivarVelasNuevas();
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

        if (velasNuevasColocadas >= velasNuevas.Length)
        {
            CompletarMinijuego();
        }
    }

    void CompletarMinijuego()
    {
        if (textoEstado != null)
            textoEstado.text = "Minijuego completado";

        Debug.Log("Minijuego de velas completado");

        // Aquí puedes:
        // - cerrar el minijuego
        // - dar una llave
        // - activar otra fase
        // - volver al juego principal
    }
}