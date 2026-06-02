using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TareaManager : MonoBehaviour
{
    public static TareaManager Instance;

    [System.Serializable]
    public class Tarea
    {
        public string id;
        public string nombre;
        public string escenaDestino;
        public bool completada = false;
        public GameObject objetoEnEscena;
    }

    public List<Tarea> tareasPosibles = new List<Tarea>();
    public int minTareasPorNoche = 1;
    public int maxTareasPorNoche = 2;
    public UIManager uiManager;

    public static System.Action OnPrimeraTareaActivada;

    private Tarea tareaActual;
    private int nocheActual = 1;
    private bool generacionTareasPausada = false;
    private bool primeraTareaYaNotificada = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Invoke("BuscarObjetosEnEscenaActual", 0.2f);

    void BuscarObjetosEnEscenaActual()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        foreach (Tarea tarea in tareasPosibles)
        {
            if (tarea.escenaDestino == escenaActual)
            {
                GameObject encontrado = GameObject.FindGameObjectWithTag($"Tarea{tarea.id}");
                if (encontrado != null)
                {
                    tarea.objetoEnEscena = encontrado;
                    if (tareaActual != null && tareaActual.id == tarea.id && !tareaActual.completada)
                        ActivarInteractuabilidadEnObjeto(encontrado);
                }
            }
        }
    }

    void ActivarInteractuabilidadEnObjeto(GameObject obj)
    {
        AbrirMinijuego interactuable = obj.GetComponent<AbrirMinijuego>();
        if (interactuable != null)
            interactuable.SetPuedeInteractuar(true);
    }

    public void IniciarNoche(int noche)
    {
        if (generacionTareasPausada)
        {
            Debug.Log("Generación de tareas pausada");
            return;
        }

        nocheActual = noche;
        int numTareas = Random.Range(minTareasPorNoche, maxTareasPorNoche + 1);

        foreach (Tarea t in tareasPosibles) t.completada = false;

        List<Tarea> disponibles = new List<Tarea>(tareasPosibles);
        List<Tarea> seleccionadas = new List<Tarea>();

        for (int i = 0; i < numTareas && disponibles.Count > 0; i++)
        {
            int idx = Random.Range(0, disponibles.Count);
            seleccionadas.Add(disponibles[idx]);
            disponibles.RemoveAt(idx);
        }

        if (seleccionadas.Count > 0) ActivarTarea(seleccionadas[0]);
        else Debug.Log("No hay tareas disponibles");
    }

    void ActivarTarea(Tarea tarea)
    {
        tareaActual = tarea;

        if (uiManager != null)
            uiManager.MostrarAvisoTarea(true, tareaActual.id);

        if (tarea.objetoEnEscena != null)
            ActivarInteractuabilidadEnObjeto(tarea.objetoEnEscena);

        if (!primeraTareaYaNotificada && nocheActual == 1)
        {
            primeraTareaYaNotificada = true;
            Debug.Log("📞 Primera tarea activada - Llamando al jefe...");
            OnPrimeraTareaActivada?.Invoke();
        }

        Debug.Log($"❗ Tarea activada: {tarea.nombre} en {tarea.escenaDestino}");
    }

    public bool PuedeInteractuarConObjeto(GameObject objeto)
    {
        if (tareaActual == null || tareaActual.completada) return false;
        string escenaActual = SceneManager.GetActiveScene().name;
        if (tareaActual.escenaDestino != escenaActual) return false;
        return tareaActual.objetoEnEscena == objeto;
    }

    public void CompletarTareaActual()
    {
        if (tareaActual == null || tareaActual.completada) return;
        tareaActual.completada = true;

        if (tareaActual.objetoEnEscena != null)
        {
            AbrirMinijuego interactuable = tareaActual.objetoEnEscena.GetComponent<AbrirMinijuego>();
            if (interactuable != null) interactuable.SetPuedeInteractuar(false);
        }

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null) gestion.CompletarTarea();

        Tarea siguiente = null;
        foreach (Tarea t in tareasPosibles)
        {
            if (!t.completada && t != tareaActual) { siguiente = t; break; }
        }

        if (siguiente != null) ActivarTarea(siguiente);
        else
        {
            tareaActual = null;
            if (uiManager != null) uiManager.MostrarAvisoTarea(false, "");
            Debug.Log("✅ Todas las tareas de la noche completadas");
        }
    }

    public void PausarGeneracionTareas(bool pausar) => generacionTareasPausada = pausar;

    public void ReiniciarParaNuevaPartida()
    {
        primeraTareaYaNotificada = false;
        tareaActual = null;
    }
}