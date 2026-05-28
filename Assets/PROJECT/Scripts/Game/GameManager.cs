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

    [Header("Escenas de juego (todas las que tengan menú de pausa)")]
    public List<string> gameSceneNames = new List<string> { "Garita", "Cementerio", "Afueras" };

    [Header("Sonido Hover")]
    public AudioClip sonidoHover;

    [Header("Cursor personalizado")]
    public Color colorCursor = Color.white;
    public int tamañoCursor = 32;
    public float radioCursor = 14f;

    private MouseLook360 mouseLook;
    private ReproductorSonidoHover reproductorHover;
    private VisualElement pauseRoot;
    private VisualElement mainRoot;
    private bool isGameScene = false;
    private Texture2D cursorPersonalizado;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Crear cursor personalizado
        CrearCursorCircularBlanco();

        // Configurar el reproductor de hover UNA SOLA VEZ
        ConfigurarAudioHover();
    }

    void CrearCursorCircularBlanco()
    {
        // Crear textura circular
        cursorPersonalizado = new Texture2D(tamañoCursor, tamañoCursor, TextureFormat.RGBA32, false);

        for (int y = 0; y < tamañoCursor; y++)
        {
            for (int x = 0; x < tamañoCursor; x++)
            {
                float dist = Vector2.Distance(new Vector2(tamañoCursor / 2, tamañoCursor / 2), new Vector2(x, y));

                if (dist < radioCursor)
                {
                    cursorPersonalizado.SetPixel(x, y, colorCursor);
                }
                else
                {
                    cursorPersonalizado.SetPixel(x, y, Color.clear);
                }
            }
        }

        cursorPersonalizado.Apply();
        Debug.Log("Cursor circular blanco creado");
    }

    void AplicarCursorPersonalizado()
    {
        if (cursorPersonalizado != null)
        {
            Vector2 hotspot = new Vector2(tamañoCursor / 2, tamañoCursor / 2);
            Cursor.SetCursor(cursorPersonalizado, hotspot, CursorMode.Auto);
            Debug.Log("Cursor personalizado aplicado");
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Limpiar referencias anteriores
        pauseRoot = null;
        mainRoot = null;

        // Verificar si es una escena de juego
        isGameScene = gameSceneNames.Contains(scene.name);

        // Buscar MouseLook en la cámara (solo en escenas de juego)
        Camera mainCam = Camera.main;
        if (mainCam != null) mouseLook = mainCam.GetComponent<MouseLook360>();

        // Configurar según la escena cargada
        if (scene.name == mainMenuSceneName)
        {
            BuscarYConfigurarMainMenu();
            SetState(GameState.MainMenu);
        }
        else if (isGameScene)
        {
            BuscarYConfigurarPauseMenu();
            // IMPORTANTE: Al cargar una nueva escena de juego, volvemos a estado Playing
            SetState(GameState.Playing);
        }
    }

    void ConfigurarAudioHover()
    {
        // Crear el reproductor de hover en el GameManager
        reproductorHover = GetComponent<ReproductorSonidoHover>();
        if (reproductorHover == null)
            reproductorHover = gameObject.AddComponent<ReproductorSonidoHover>();
        reproductorHover.sonidoHover = sonidoHover;
    }

    void BuscarYConfigurarMainMenu()
    {
        // Buscar el UIDocument del menú principal
        var documentos = FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
        foreach (var doc in documentos)
        {
            var root = doc.rootVisualElement;
            Button jugar = root.Q<Button>("BotonJugar");
            Button salir = root.Q<Button>("BotonSalir");

            if (jugar != null && salir != null)
            {
                mainRoot = root;

                // Configurar hover y clicks
                ConfigurarBotonConHover(jugar, StartGame);
                ConfigurarBotonConHover(salir, QuitGame);

                // Asegurar que se vea
                mainRoot.style.display = DisplayStyle.Flex;

                Debug.Log("Menú principal configurado correctamente");
                break;
            }
        }
    }

    void BuscarYConfigurarPauseMenu()
    {
        // Buscar el UIDocument del menú de pausa en la escena actual
        var documentos = FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
        foreach (var doc in documentos)
        {
            var root = doc.rootVisualElement;
            Button continuar = root.Q<Button>("BotonContinuar");
            Button salirMenu = root.Q<Button>("BotonSalirMenu");

            if (continuar != null && salirMenu != null)
            {
                pauseRoot = root;

                // Configurar hover y clicks
                ConfigurarBotonConHover(continuar, ResumeGame);
                ConfigurarBotonConHover(salirMenu, BackToMainMenu);

                // Ocultar al inicio (cuando se está jugando)
                pauseRoot.style.display = DisplayStyle.None;

                Debug.Log($"Menú de pausa configurado en escena: {SceneManager.GetActiveScene().name}");
                break;
            }
        }

        // Si no se encontró el menú de pausa, mostrar advertencia
        if (pauseRoot == null)
        {
            Debug.LogWarning($"No se encontró menú de pausa en la escena {SceneManager.GetActiveScene().name}. Asegúrate de que haya un UIDocument con botones 'BotonContinuar' y 'BotonSalirMenu'");
        }
    }

    void ConfigurarBotonConHover(Button boton, System.Action accion)
    {
        if (boton == null) return;

        // Limpiar eventos anteriores para evitar duplicados
        boton.clicked -= accion;
        boton.clicked += accion;

        // Limpiar hover anterior
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
        // Detectar Escape SOLO en escenas de juego y cuando está jugando
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
                AplicarCursorPersonalizado(); // 👈 Cursor blanco circular
                if (mouseLook) mouseLook.enabled = false;
                if (pauseRoot != null) pauseRoot.style.display = DisplayStyle.None;
                if (mainRoot != null) mainRoot.style.display = DisplayStyle.Flex;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false; // Cursor invisible mientras juegas (FPS)
                if (mouseLook) mouseLook.enabled = true;
                if (pauseRoot != null) pauseRoot.style.display = DisplayStyle.None;
                if (mainRoot != null) mainRoot.style.display = DisplayStyle.None;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                AplicarCursorPersonalizado(); // 👈 Cursor blanco circular
                if (mouseLook) mouseLook.enabled = false;
                if (pauseRoot != null) pauseRoot.style.display = DisplayStyle.Flex;
                if (mainRoot != null) mainRoot.style.display = DisplayStyle.None;
                break;
        }

        Debug.Log($"Estado cambiado a: {newState}");
    }

    // ============ MÉTODOS PÚBLICOS PARA LOS BOTONES ============

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneNames[0]); // Carga la primera escena de juego (Garita)
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