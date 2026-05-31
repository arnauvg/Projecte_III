using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Mapa Logo")]
    public Image mapaLogo;
    public Sprite mapaNormal;
    public Sprite mapaNotificacion;

    [Header("Panel Mapa")]
    public GameObject panelMapa;

    [Header("Botones del mapa (Images)")]
    public Image botonGarita;
    public Image botonAfueras;
    public Image botonCripta;
    public Image botonTumbas;

    [Header("Sprites normales")]
    public Sprite garitaNormal;
    public Sprite afuerasNormal;
    public Sprite criptaNormal;
    public Sprite tumbasNormal;

    [Header("Sprites con notificación")]
    public Sprite garitaNotificacion;
    public Sprite afuerasNotificacion;
    public Sprite criptaNotificacion;
    public Sprite tumbasNotificacion;

    [Header("Texto contador")]
    public TextMeshProUGUI textoVisitantes;

    private bool tareaPendiente = false;
    private string tareaUbicacion = "";

    void Start()
    {
        if (mapaLogo != null) mapaLogo.sprite = mapaNormal;
        if (panelMapa != null) panelMapa.SetActive(false);
        ResetearBotonesMapa();
    }

    public void MostrarAvisoTarea(bool mostrar, string ubicacion)
    {
        tareaPendiente = mostrar;
        tareaUbicacion = ubicacion;

        Debug.Log($"📢 UIManager: tarea={(mostrar ? ubicacion : "ninguna")}");

        // Cambiar ícono del mapa en HUD
        if (mapaLogo != null)
            mapaLogo.sprite = mostrar ? mapaNotificacion : mapaNormal;

        // Siempre actualizar botones (aunque el mapa esté cerrado)
        ActualizarBotonesMapa();
    }

    public void AbrirMapa()
    {
        if (panelMapa != null)
        {
            ActualizarBotonesMapa(); // por si acaso
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
            string ub = tareaUbicacion.ToLower();

            // Mapear "velas" o "pila" a Cripta
            if (ub == "velas" || ub == "pila")
            {
                if (botonCripta != null && criptaNotificacion != null)
                {
                    botonCripta.sprite = criptaNotificacion;
                    Debug.Log($"✅ Botón Cripta notificación (tarea: {ub})");
                }
            }
            else
            {
                switch (ub)
                {
                    case "garita":
                        if (botonGarita != null) botonGarita.sprite = garitaNotificacion;
                        break;
                    case "afueras":
                        if (botonAfueras != null) botonAfueras.sprite = afuerasNotificacion;
                        break;
                    case "tumbas":
                        if (botonTumbas != null) botonTumbas.sprite = tumbasNotificacion;
                        break;
                }
            }
        }
    }

    void ResetearBotonesMapa()
    {
        if (botonGarita != null && garitaNormal != null) botonGarita.sprite = garitaNormal;
        if (botonAfueras != null && afuerasNormal != null) botonAfueras.sprite = afuerasNormal;
        if (botonCripta != null && criptaNormal != null) botonCripta.sprite = criptaNormal;
        if (botonTumbas != null && tumbasNormal != null) botonTumbas.sprite = tumbasNormal;
    }

    public void MarcarTareaCompletada()
    {
        tareaPendiente = false;
        tareaUbicacion = "";
        if (mapaLogo != null) mapaLogo.sprite = mapaNormal;
        ResetearBotonesMapa();
    }

    public void ActualizarContadorVisitantes(int actual, int total)
    {
        if (textoVisitantes != null)
            textoVisitantes.text = $"Visitantes: {actual}/{total}";
    }
}