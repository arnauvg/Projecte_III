using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    private HashSet<string> tareasActivas = new HashSet<string>();
    private Canvas canvasPadre;

    void Start()
    {
        InicializarUI();
    }

    void InicializarUI()
    {
        Debug.Log("UIManager: Inicializando UI...");

        // 🔥 Buscar el Canvas padre
        canvasPadre = GetComponentInParent<Canvas>();
        if (canvasPadre == null)
        {
            canvasPadre = FindFirstObjectByType<Canvas>();
        }

        if (canvasPadre != null)
        {
            canvasPadre.gameObject.SetActive(true);
            canvasPadre.enabled = true;
            Debug.Log($"Canvas encontrado: {canvasPadre.name}");
        }
        else
        {
            Debug.LogError("No se encontró Canvas. Creando uno...");
            GameObject canvasGO = new GameObject("PersistentCanvas");
            canvasPadre = canvasGO.AddComponent<Canvas>();
            canvasPadre.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            canvasGO.transform.SetParent(transform);
            Debug.Log("Canvas creado por código");
        }

        // Buscar referencias si no están asignadas
        if (mapaLogo == null)
        {
            Transform interficie = transform.Find("UI/Interficie");
            if (interficie != null)
                mapaLogo = interficie.Find("MapaLogo")?.GetComponent<Image>();
        }

        if (panelMapa == null)
        {
            Transform mapa = transform.Find("UI/Mapa");
            if (mapa != null)
                panelMapa = mapa.Find("panelMapa")?.gameObject;
        }

        if (panelMapa != null)
        {
            if (botonGarita == null)
            {
                Transform garita = panelMapa.transform.Find("garita");
                if (garita != null) botonGarita = garita.GetComponent<Image>();
            }
            if (botonAfueras == null)
            {
                Transform afueras = panelMapa.transform.Find("afueras");
                if (afueras != null) botonAfueras = afueras.GetComponent<Image>();
            }
            if (botonCripta == null)
            {
                Transform cripta = panelMapa.transform.Find("cripta");
                if (cripta != null) botonCripta = cripta.GetComponent<Image>();
            }
            if (botonTumbas == null)
            {
                Transform tumbas = panelMapa.transform.Find("tumbas");
                if (tumbas != null) botonTumbas = tumbas.GetComponent<Image>();
            }
        }

        if (mapaLogo != null)
            mapaLogo.sprite = mapaNormal;

        if (panelMapa != null)
            panelMapa.SetActive(false);

        ResetearBotonesMapa();

        Debug.Log($"UIManager: UI inicializada. mapaLogo={mapaLogo != null}, panelMapa={panelMapa != null}, canvas={canvasPadre != null}");
    }

    public void AgregarTareaActiva(string ubicacion)
    {
        if (string.IsNullOrEmpty(ubicacion)) return;

        tareasActivas.Add(ubicacion);
        Debug.Log($"📢 UIManager: Tarea agregada - {ubicacion}. Total activas: {tareasActivas.Count}");

        ActualizarIconoMapa();
        ActualizarBotonesMapa();
    }

    public void EliminarTareaActiva(string ubicacion)
    {
        if (string.IsNullOrEmpty(ubicacion)) return;

        tareasActivas.Remove(ubicacion);
        Debug.Log($"📢 UIManager: Tarea eliminada - {ubicacion}. Total activas: {tareasActivas.Count}");

        ActualizarIconoMapa();
        ActualizarBotonesMapa();
    }

    void ActualizarIconoMapa()
    {
        if (mapaLogo != null)
        {
            mapaLogo.sprite = (tareasActivas.Count > 0) ? mapaNotificacion : mapaNormal;
        }
    }

    public void AbrirMapa()
    {
        if (panelMapa != null)
        {
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

        foreach (string ub in tareasActivas)
        {
            string ubicacionLower = ub.ToLower();

            if (ubicacionLower == "velas" || ubicacionLower == "pila")
            {
                if (botonCripta != null && criptaNotificacion != null)
                {
                    botonCripta.sprite = criptaNotificacion;
                }
            }
            else
            {
                switch (ubicacionLower)
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
        tareasActivas.Clear();
        if (mapaLogo != null) mapaLogo.sprite = mapaNormal;
        ResetearBotonesMapa();
    }

    public void ActualizarContadorVisitantes(int actual, int total)
    {
        if (textoVisitantes != null)
            textoVisitantes.text = $"Visitantes: {actual}/{total}";
    }
}