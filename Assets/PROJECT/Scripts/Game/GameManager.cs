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
    public Color colorCursor = Color.white;
    public int tamañoCursor = 32;
    public float radioCursor = 14f;

    private MouseLook360 mouseLook;
    private ReproductorSonidoHover reproductorHover;
    private VisualElement pauseRoot;
    private VisualElement mainRoot;
    private bool isGameScene = false;
    private Texture2D cursorPersonalizado;
    private UIDocument pauseUIDocument;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CrearCursorCircularBlanco();
        ConfigurarAudioHover();
    }

    void CrearCursorCircularBlanco()
    {
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
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneLoaded: {scene.name}");

        // Limpiar referencias anteriores
        pauseRoot = null;
        mainRoot = null;
        pauseUIDocument = null;
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
        Debug.Log("Buscando menú principal...");

        var documentos = FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
        foreach (var doc in documentos)
        {
            if (doc.gameObject.name == "MainMenuUI")
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
    }

    void BuscarYConfigurarPauseMenu()
    {
        Debug.Log("Buscando menú de pausa en la escena actual...");

        // 🔥 Buscar PauseMenuUI en la escena actual
        GameObject pauseMenuGO = GameObject.Find("PauseMenuUI");

        if (pauseMenuGO == null)
        {
            Debug.LogWarning($"No se encontró 'PauseMenuUI' en la escena {SceneManager.GetActiveScene().name}");
            return;
        }

        pauseUIDocument = pauseMenuGO.GetComponent<UIDocument>();

        if (pauseUIDocument == null)
        {
            Debug.LogWarning("No se encontró UIDocument en 'PauseMenuUI'");
            return;
        }

        var root = pauseUIDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogWarning("rootVisualElement es null");
            return;
        }

        Button continuar = root.Q<Button>("BotonContinuar");
        Button salirMenu = root.Q<Button>("BotonSalirMenu");

        if (continuar != null && salirMenu != null)
        {
            pauseRoot = root;
            ConfigurarBotonConHover(continuar, ResumeGame);
            ConfigurarBotonConHover(salirMenu, BackToMainMenu);
            pauseRoot.style.display = DisplayStyle.None;
            Debug.Log($"Menú de pausa configurado correctamente en escena: {SceneManager.GetActiveScene().name}");
        }
        else
        {
            Debug.LogWarning($"No se encontraron los botones. Continuar: {(continuar != null)}, Salir: {(salirMenu != null)}");
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
                Debug.Log("ESC presionado - Cambiando a estado Paused");
                SetState(GameState.Paused);
            }
        }
    }

    public void SetState(GameState newState)
    {
        Debug.Log($"SetState llamado: {currentState} -> {newState}");
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

                if (pauseRoot != null)
                {
                    pauseRoot.style.display = DisplayStyle.Flex;
                    Debug.Log("Menú de pausa mostrado");
                }
                else
                {
                    Debug.LogError("pauseRoot es NULL - no se puede mostrar el menú");
                }

                if (mainRoot != null) mainRoot.style.display = DisplayStyle.None;
                break;
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame llamado - Cargando primera escena");
        SceneManager.LoadScene(gameSceneNames[0]);
    }

    public void ResumeGame()
    {
        Debug.Log("ResumeGame llamado");
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
        Debug.Log("BackToMainMenu llamado");
        SceneManager.LoadScene(mainMenuSceneName);
        SetState(GameState.MainMenu);
    }
}