using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TareaManager : MonoBehaviour
{
    public static TareaManager Instance;
    [Header("Configuración de spawn")]
    public int minTareasPorNoche = 1;
    public int maxTareasPorNoche = 2;
    private bool yaSeAsignoTareaEstaNoche = false;

    [System.Serializable]
    public class Tarea
    {
        public string id;
        public string nombre;
        public string escenaDestino;
        public bool completada = false;
        public GameObject objetoEnEscena;
    }

    [Header("Tareas disponibles")]
    public List<Tarea> tareasPosibles = new List<Tarea>();

    [Header("Referencias UI")]
    public UIManager uiManager;

    private Tarea tareaActual;
    private bool tareaActiva = false;
    private int nocheActual = 1;

    // Guarda las tareas que ya han salido en noches anteriores
    private List<string> tareasUsadas = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("TareaManager: Instancia creada");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Invoke(nameof(BuscarObjetosEnEscenaActual), 0.2f);
    }

    void BuscarObjetosEnEscenaActual()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        foreach (Tarea tarea in tareasPosibles)
        {
            if (tarea.escenaDestino == escenaActual)
            {
                string tagBuscado = $"Tarea{tarea.id}";
                GameObject encontrado = GameObject.FindGameObjectWithTag(tagBuscado);

                if (encontrado != null)
                {
                    tarea.objetoEnEscena = encontrado;
                    Debug.Log($"Encontrado objeto de tarea: {encontrado.name} para tarea {tarea.id}");

                    if (tareaActual != null && tareaActual.id == tarea.id && !tareaActual.completada)
                    {
                        ActivarInteractuabilidadEnObjeto(encontrado);
                    }
                    else
                    {
                        DesactivarInteractuabilidadEnObjeto(encontrado);
                    }
                }
            }
        }
    }

    void ActivarInteractuabilidadEnObjeto(GameObject obj)
    {
        AbrirMinijuego interactuable = obj.GetComponent<AbrirMinijuego>();
        if (interactuable != null)
        {
            interactuable.SetPuedeInteractuar(true);
            Debug.Log($"Interactuabilidad activada para: {obj.name}");
        }
    }

    void DesactivarInteractuabilidadEnObjeto(GameObject obj)
    {
        AbrirMinijuego interactuable = obj.GetComponent<AbrirMinijuego>();
        if (interactuable != null)
        {
            interactuable.SetPuedeInteractuar(false);
        }
    }

    public void IniciarNoche(int noche)
    {
        nocheActual = noche;
        yaSeAsignoTareaEstaNoche = false;

        foreach (Tarea tarea in tareasPosibles)
        {
            tarea.completada = false;

            if (tarea.objetoEnEscena != null)
            {
                DesactivarInteractuabilidadEnObjeto(tarea.objetoEnEscena);
            }
        }

        AsignarUnaSolaTarea();
    }

    void ActivarTarea(Tarea tarea)
    {
        tareaActual = tarea;
        tareaActiva = true;

        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();

        if (uiManager != null)
            uiManager.MostrarAvisoTarea(true, tareaActual.id);

        Debug.Log($"Tarea activada: {tarea.nombre} en {tarea.escenaDestino}");

        if (tarea.objetoEnEscena != null)
        {
            ActivarInteractuabilidadEnObjeto(tarea.objetoEnEscena);
        }
    }

    public bool PuedeInteractuarConObjeto(GameObject objeto)
    {
        if (tareaActual == null) return false;
        if (tareaActual.completada) return false;

        string escenaActual = SceneManager.GetActiveScene().name;
        if (tareaActual.escenaDestino != escenaActual) return false;

        return tareaActual.objetoEnEscena == objeto;
    }

    public void CompletarTareaActual()
    {
        if (tareaActual == null) return;
        if (tareaActual.completada) return;

        tareaActual.completada = true;
        tareaActiva = false;

        if (tareaActual.objetoEnEscena != null)
        {
            DesactivarInteractuabilidadEnObjeto(tareaActual.objetoEnEscena);
        }

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
            gestion.CompletarTarea();

        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();

        if (uiManager != null)
            uiManager.MostrarAvisoTarea(false, "");

        Debug.Log("Tarea completada: " + tareaActual.nombre);

        tareaActual = null;
    }
    private void AsignarUnaSolaTarea()
    {
        if (yaSeAsignoTareaEstaNoche)
        {
            Debug.Log("Ya hay una tarea asignada esta noche. No se asigna otra.");
            return;
        }

        if (tareasPosibles.Count == 0)
        {
            Debug.LogWarning("No hay tareas disponibles.");
            return;
        }

        int indice = Random.Range(0, tareasPosibles.Count);
        Tarea tareaSeleccionada = tareasPosibles[indice];

        ActivarTarea(tareaSeleccionada);

        yaSeAsignoTareaEstaNoche = true;

        Debug.Log("Tarea única asignada esta noche: " + tareaSeleccionada.nombre);
    }

    public void ReiniciarJuegoCompleto()
    {
        tareasUsadas.Clear();
        tareaActual = null;
        tareaActiva = false;
        nocheActual = 1;

        foreach (Tarea tarea in tareasPosibles)
        {
            tarea.completada = false;
        }

        if (uiManager != null)
            uiManager.MostrarAvisoTarea(false, "");
    }
}