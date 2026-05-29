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

    [Header("Texto contador")]
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

        Debug.Log($"📢 UIManager: MostrarAvisoTarea - mostrar={mostrar}, ubicacion={ubicacion}");

        // Cambiar ícono del mapa en HUD
        if (mapaLogo != null)
        {
            mapaLogo.sprite = mostrar ? mapaNotificacion : mapaNormal;
        }

        // IMPORTANTE: Actualizar botones ahora aunque el mapa NO esté abierto
        // Así cuando se abra, ya estarán actualizados
        ActualizarBotonesMapa();
    }

    public void AbrirMapa()
    {
        Debug.Log($"🗺️ Abriendo mapa - Tarea pendiente: {tareaPendiente}, ubicación: {tareaUbicacion}");

        if (panelMapa != null)
        {
            // Actualizar botones antes de mostrar (por si acaso)
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
        Debug.Log($"🎨 Actualizando botones del mapa - tareaPendiente={tareaPendiente}, ubicacion={tareaUbicacion}");

        // Primero resetear todos a normal
        ResetearBotonesMapa();

        // Si hay tarea pendiente, cambiar el botón correspondiente
        if (tareaPendiente && !string.IsNullOrEmpty(tareaUbicacion))
        {
            string ubicacionLower = tareaUbicacion.ToLower();
            Debug.Log($"Buscando botón para ubicación: {ubicacionLower}");

            switch (ubicacionLower)
            {
                case "garita":
                    if (botonGarita != null && garitaNotificacion != null)
                    {
                        botonGarita.sprite = garitaNotificacion;
                        Debug.Log("✅ Botón Garita actualizado a notificación");
                    }
                    else
                    {
                        Debug.LogWarning($"Botón Garita o sprite notificación es NULL. botonGarita={botonGarita != null}, garitaNotificacion={garitaNotificacion != null}");
                    }
                    break;
                case "afueras":
                    if (botonAfueras != null && afuerasNotificacion != null)
                    {
                        botonAfueras.sprite = afuerasNotificacion;
                        Debug.Log("✅ Botón Afueras actualizado a notificación");
                    }
                    else
                    {
                        Debug.LogWarning($"Botón Afueras o sprite notificación es NULL. botonAfueras={botonAfueras != null}, afuerasNotificacion={afuerasNotificacion != null}");
                    }
                    break;
                case "cripta":
                    if (botonCripta != null && criptaNotificacion != null)
                    {
                        botonCripta.sprite = criptaNotificacion;
                        Debug.Log("✅ Botón Cripta actualizado a notificación");
                    }
                    else
                    {
                        Debug.LogWarning($"Botón Cripta o sprite notificación es NULL. botonCripta={botonCripta != null}, criptaNotificacion={criptaNotificacion != null}");
                    }
                    break;
                case "tumbas":
                    if (botonTumbas != null && tumbasNotificacion != null)
                    {
                        botonTumbas.sprite = tumbasNotificacion;
                        Debug.Log("✅ Botón Tumbas actualizado a notificación");
                    }
                    else
                    {
                        Debug.LogWarning($"Botón Tumbas o sprite notificación es NULL. botonTumbas={botonTumbas != null}, tumbasNotificacion={tumbasNotificacion != null}");
                    }
                    break;
                default:
                    Debug.LogWarning($"Ubicación no reconocida: {ubicacionLower}");
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
        Debug.Log("Resetear botones del mapa a normales");

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

        Debug.Log("✅ Tarea marcada como completada en UIManager");
    }

    public void ActualizarContadorVisitantes(int actual, int total)
    {
        if (textoVisitantes != null)
            textoVisitantes.text = $"Visitantes: {actual}/{total}";
    }
}