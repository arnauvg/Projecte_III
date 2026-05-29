using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    [Header("Texto contador (opcional)")]
    public TextMeshProUGUI textoVisitantes;

    private bool tareaPendiente = false;
    private string tareaUbicacion = "";

    void Start()
    {
        if (mapaLogo != null)
            mapaLogo.sprite = mapaNormal;

        if (panelMapa != null)
            panelMapa.SetActive(false);

        ResetearBotonesMapa();
    }

    public void MostrarAvisoTarea(bool mostrar, string ubicacion)
    {
        tareaPendiente = mostrar;
        tareaUbicacion = ubicacion;

        Debug.Log($"UIManager: MostrarAvisoTarea - mostrar={mostrar}, ubicacion={ubicacion}");

        // Cambiar ícono del mapa en HUD
        if (mapaLogo != null)
        {
            mapaLogo.sprite = mostrar ? mapaNotificacion : mapaNormal;
        }

        // Si el mapa está abierto, actualizar botones inmediatamente
        if (panelMapa != null && panelMapa.activeSelf)
        {
            ActualizarBotonesMapa();
        }
    }

    public void AbrirMapa()
    {
        if (panelMapa != null)
        {
            // Actualizar botones antes de mostrar
            ActualizarBotonesMapa();

            panelMapa.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;

            Debug.Log("UIManager: Mapa abierto");
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

            Debug.Log("UIManager: Mapa cerrado");
        }
    }

    void ActualizarBotonesMapa()
    {
        Debug.Log($"UIManager: Actualizando botones - tareaPendiente={tareaPendiente}, ubicacion={tareaUbicacion}");

        // Primero resetear todos a normal
        ResetearBotonesMapa();

        // Si hay tarea pendiente, cambiar el botón correspondiente
        if (tareaPendiente && !string.IsNullOrEmpty(tareaUbicacion))
        {
            switch (tareaUbicacion.ToLower())
            {
                case "garita":
                    if (botonGarita != null && garitaNotificacion != null)
                    {
                        botonGarita.sprite = garitaNotificacion;
                        Debug.Log("✅ Botón Garita actualizado a notificación");
                    }
                    break;
                case "afueras":
                    if (botonAfueras != null && afuerasNotificacion != null)
                    {
                        botonAfueras.sprite = afuerasNotificacion;
                        Debug.Log("✅ Botón Afueras actualizado a notificación");
                    }
                    break;
                case "cripta":
                    if (botonCripta != null && criptaNotificacion != null)
                    {
                        botonCripta.sprite = criptaNotificacion;
                        Debug.Log("✅ Botón Cripta actualizado a notificación");
                    }
                    break;
                case "tumbas":
                    if (botonTumbas != null && tumbasNotificacion != null)
                    {
                        botonTumbas.sprite = tumbasNotificacion;
                        Debug.Log("✅ Botón Tumbas actualizado a notificación");
                    }
                    break;
                default:
                    Debug.LogWarning($"Ubicación no reconocida: {tareaUbicacion}");
                    break;
            }
        }
        else
        {
            Debug.Log("No hay tarea pendiente, botones normales");
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

        Debug.Log("Botones del mapa reseteados a normales");
    }

    public void MarcarTareaCompletada()
    {
        tareaPendiente = false;
        tareaUbicacion = "";

        if (mapaLogo != null)
            mapaLogo.sprite = mapaNormal;

        ResetearBotonesMapa();

        Debug.Log("UIManager: Tarea marcada como completada");
    }

    public void ReiniciarContadores()
    {
        if (textoVisitantes != null)
            textoVisitantes.text = "Visitantes: 0/3";
        tareaPendiente = false;
        tareaUbicacion = "";
        ResetearBotonesMapa();
    }

    public void ActualizarContadorVisitantes(int actual, int total)
    {
        if (textoVisitantes != null)
            textoVisitantes.text = $"Visitantes: {actual}/{total}";
    }
}