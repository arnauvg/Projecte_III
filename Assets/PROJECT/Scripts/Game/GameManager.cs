using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused }
    public GameState currentState = GameState.MainMenu;

    [Header("Escenas")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Escenas de juego")]
    public List<string> gameSceneNames = new List<string> { "Garita", "Cementerio", "Afueras" };

    [Header("Sonido Hover")]
    public AudioClip sonidoHover;

    [Header("Cursor personalizado")]
    public Texture2D cursorSprite;        // ← Arrastra tu sprite aquí
    public Vector2 hotspot = new Vector2(0, 0);  // ← Punto de clic (0,0 = esquina superior izquierda)
    public CursorMode cursorMode = CursorMode.Auto;

    private MouseLook360 mouseLook;
    private ReproductorSonidoHover reproductorHover;
    private VisualElement pauseRoot;
    private VisualElement mainRoot;
    private bool isGameScene = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Configurar el reproductor de hover
        ConfigurarAudioHover();
    }

    void AplicarCursorPersonalizado()
    {
        if (cursorSprite != null)
        {
            Cursor.SetCursor(cursorSprite, hotspot, cursorMode);
            Debug.Log($"Cursor personalizado aplicado: {cursorSprite.name}");
        }
        else
        {
            Debug.LogWarning("No hay sprite asignado para el cursor");
        }
    }

    // ... el resto del script igual (OnEnable, OnDisable, OnSceneLoaded, etc.)

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        pauseRoot = null;
        mainRoot = null;
        isGameScene = gameSceneNames.Contains(scene.name);

        Camera mainCam = Camera.main;
        if (mainCam != null) mouseLook = mainCam.GetComponent<MouseLook360>();

        if (scene.name == mainMenuSceneName)
        {
            BuscarYConfigurarMainMenu();
            SetState(GameState.MainMenu);
        }
        else if (isGameScene)
        {
            BuscarYConfigurarPauseMenu();
            SetState(GameState.Playing);
        }
    }

    void ConfigurarAudioHover()
    {
        reproductorHover = GetComponent<ReproductorSonidoHover>();
        if (reproductorHover == null)
            reproductorHover = gameObject.AddComponent<ReproductorSonidoHover>();
        reproductorHover.sonidoHover = sonidoHover;
    }

    void BuscarYConfigurarMainMenu()
    {
        var documentos = FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
        foreach (var doc in documentos)
        {
            var root = doc.rootVisualElement;
            Button jugar = root.Q<Button>("BotonJugar");
            Button salir = root.Q<Button>("BotonSalir");

            if (jugar != null && salir != null)
            {
                mainRoot = root;
                ConfigurarBotonConHover(jugar, StartGame);
                ConfigurarBotonConHover(salir, QuitGame);
                mainRoot.style.display = DisplayStyle.Flex;
                Debug.Log("Menú principal configurado correctamente");
                break;
            }
        }
    }

    void BuscarYConfigurarPauseMenu()
    {
        var documentos = FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
        foreach (var doc in documentos)
        {
            var root = doc.rootVisualElement;
            Button continuar = root.Q<Button>("BotonContinuar");
            Button salirMenu = root.Q<Button>("BotonSalirMenu");

            if (continuar != null && salirMenu != null)
            {
                pauseRoot = root;
                ConfigurarBotonConHover(continuar, ResumeGame);
                ConfigurarBotonConHover(salirMenu, BackToMainMenu);
                pauseRoot.style.display = DisplayStyle.None;
                Debug.Log($"Menú de pausa configurado en escena: {SceneManager.GetActiveScene().name}");
                break;
            }
        }

        if (pauseRoot == null)
        {
            Debug.LogWarning($"No se encontró menú de pausa en la escena {SceneManager.GetActiveScene().name}");
        }
    }

    void ConfigurarBotonConHover(Button boton, System.Action accion)
    {
        if (boton == null) return;
        boton.clicked -= accion;
        boton.clicked += accion;
        boton.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
        boton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
    }

    void OnMouseEnter(MouseEnterEvent ev)
    {
        if (reproductorHover != null && sonidoHover != null)
            reproductorHover.ReproducirHover();
    }

    private void Update()
    {
        if (isGameScene && currentState == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetState(GameState.Paused);
            }
        }
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                AplicarCursorPersonalizado();
                if (mouseLook) mouseLook.enabled = false;
                if (pauseRoot != null) pauseRoot.style.display = DisplayStyle.None;
                if (mainRoot != null) mainRoot.style.display = DisplayStyle.Flex;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (mouseLook) mouseLook.enabled = true;
                if (pauseRoot != null) pauseRoot.style.display = DisplayStyle.None;
                if (mainRoot != null) mainRoot.style.display = DisplayStyle.None;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                AplicarCursorPersonalizado();
                if (mouseLook) mouseLook.enabled = false;
                if (pauseRoot != null) pauseRoot.style.display = DisplayStyle.Flex;
                if (mainRoot != null) mainRoot.style.display = DisplayStyle.None;
                break;
        }

        Debug.Log($"Estado cambiado a: {newState}");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneNames[0]);
    }

    public void ResumeGame()
    {
        SetState(GameState.Playing);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
        SetState(GameState.MainMenu);
    }
}