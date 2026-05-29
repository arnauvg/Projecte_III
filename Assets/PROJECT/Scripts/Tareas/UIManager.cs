using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Mapa Logo")]
    public Image mapaLogo;           // El ícono del mapa en HUD
    public Sprite mapaNormal;
    public Sprite mapaNotificacion;

    [Header("Panel Mapa")]
    public GameObject panelMapa;
    public Image botonGarita;
    public Image botonAfueras;
    public Image botonCripta;
    public Image botonTumbas;

    [Header("Sprites de botones - Normal")]
    public Sprite garitaNormal;
    public Sprite afuerasNormal;
    public Sprite criptaNormal;
    public Sprite tumbasNormal;

    [Header("Sprites de botones - Notificación")]
    public Sprite garitaNotificacion;
    public Sprite afuerasNotificacion;
    public Sprite criptaNotificacion;
    public Sprite tumbasNotificacion;

    [Header("Texto contador (opcional)")]
    public TextMeshProUGUI textoVisitantes;

    private bool tareaPendiente = false;
    private string tareaUbicacion = "";

    void Start()
    {
        // Configurar estado inicial
        if (mapaLogo != null)
            mapaLogo.sprite = mapaNormal;

        if (panelMapa != null)
            panelMapa.SetActive(false);

        // Resetear botones a estado normal
        ResetearBotonesMapa();
    }

    public void MostrarAvisoTarea(bool mostrar, string ubicacion)
    {
        tareaPendiente = mostrar;
        tareaUbicacion = ubicacion;

        // Cambiar ícono del mapa en HUD
        if (mapaLogo != null)
        {
            mapaLogo.sprite = mostrar ? mapaNotificacion : mapaNormal;
        }
    }

    public void AbrirMapa()
    {
        if (panelMapa != null)
        {
            // Actualizar botones según tarea pendiente
            ActualizarBotonesMapa();

            panelMapa.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public void CerrarMapa()
    {
        if (panelMapa != null)
        {
            panelMapa.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    void ActualizarBotonesMapa()
    {
        ResetearBotonesMapa();

        if (tareaPendiente && !string.IsNullOrEmpty(tareaUbicacion))
        {
            switch (tareaUbicacion)
            {
                case "garita":
                    if (botonGarita != null && garitaNotificacion != null)
                        botonGarita.sprite = garitaNotificacion;
                    break;
                case "afueras":
                    if (botonAfueras != null && afuerasNotificacion != null)
                        botonAfueras.sprite = afuerasNotificacion;
                    break;
                case "cripta":
                    if (botonCripta != null && criptaNotificacion != null)
                        botonCripta.sprite = criptaNotificacion;
                    break;
                case "tumbas":
                    if (botonTumbas != null && tumbasNotificacion != null)
                        botonTumbas.sprite = tumbasNotificacion;
                    break;
            }
        }
    }

    void ResetearBotonesMapa()
    {
        if (botonGarita != null && garitaNormal != null)
            botonGarita.sprite = garitaNormal;
        if (botonAfueras != null && afuerasNormal != null)
            botonAfueras.sprite = afuerasNormal;
        if (botonCripta != null && criptaNormal != null)
            botonCripta.sprite = criptaNormal;
        if (botonTumbas != null && tumbasNormal != null)
            botonTumbas.sprite = tumbasNormal;
    }

    public void MarcarTareaCompletada()
    {
        tareaPendiente = false;
        tareaUbicacion = "";

        if (mapaLogo != null)
            mapaLogo.sprite = mapaNormal;

        ResetearBotonesMapa();
    }
}