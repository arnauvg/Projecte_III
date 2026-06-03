using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    [Header("Escena a cargar")]
    public string gameSceneName = "Garita";

    [Header("Sonido Hover")]
    public AudioClip sonidoHover;

    private ReproductorSonidoHover reproductorHover;

    void Start()
    {
        ConfigurarAudioHover();

        // Resetear el teléfono para que suene en la nueva partida
        Telefono.Resetear();

        // Asegurar que no hay PersistentGameManager residual
        PersistentGameManager pgm = FindFirstObjectByType<PersistentGameManager>();
        if (pgm != null)
        {
            Destroy(pgm.gameObject);
            Debug.Log("PersistentGameManager residual destruido desde MainMenu");
        }

        UIDocument uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
            uiDoc = FindFirstObjectByType<UIDocument>();

        if (uiDoc != null)
        {
            var root = uiDoc.rootVisualElement;
            Button jugar = root.Q<Button>("BotonJugar");
            Button salir = root.Q<Button>("BotonSalir");

            if (jugar != null)
                ConfigurarBotonConHover(jugar, StartGame);
            if (salir != null)
                ConfigurarBotonConHover(salir, QuitGame);

            Debug.Log("Menú principal configurado correctamente");
        }
        else
        {
            Debug.LogError("No se encontró UIDocument en la escena");
        }
    }

    void ConfigurarAudioHover()
    {
        reproductorHover = GetComponent<ReproductorSonidoHover>();
        if (reproductorHover == null)
            reproductorHover = gameObject.AddComponent<ReproductorSonidoHover>();
        reproductorHover.sonidoHover = sonidoHover;
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

    void StartGame()
    {
        Debug.Log("Cargando escena: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}