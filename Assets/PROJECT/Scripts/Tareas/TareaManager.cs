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

    [Header("Tareas disponibles")]
    public List<Tarea> tareasPosibles = new List<Tarea>();

    [Header("Configuración de spawn")]
    public int minTareasPorNoche = 1;
    public int maxTareasPorNoche = 2;

    [Header("Referencias UI")]
    public UIManager uiManager;

    private Tarea tareaActual;
    private bool tareaActiva = false;
    private int nocheActual = 1;

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
        Debug.Log("TareaManager: Suscrito a sceneLoaded");
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
        Debug.Log($"TareaManager: Escena cargada - {scene.name}");
        Invoke("BuscarObjetosEnEscenaActual", 0.2f);
    }

    void BuscarObjetosEnEscenaActual()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        Debug.Log($"Buscando objetos en escena: {escenaActual}");

        foreach (Tarea tarea in tareasPosibles)
        {
            if (tarea.escenaDestino == escenaActual)
            {
                string tagBuscado = $"Tarea{tarea.id}";
                GameObject encontrado = GameObject.FindGameObjectWithTag(tagBuscado);

                if (encontrado != null)
                {
                    tarea.objetoEnEscena = encontrado;
                    Debug.Log($"✅ Encontrado: {encontrado.name} para tarea {tarea.id}");

                    // Si esta tarea es la actual, activar interactuabilidad
                    if (tareaActual != null && tareaActual.id == tarea.id && !tareaActual.completada)
                    {
                        ActivarInteractuabilidadEnObjeto(encontrado);
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠️ No encontrado objeto con tag '{tagBuscado}'");
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
            Debug.Log($"🔓 Interactuabilidad activada para: {obj.name}");
        }
    }

    public void IniciarNoche(int noche)
    {
        nocheActual = noche;

        int numTareas = Random.Range(minTareasPorNoche, maxTareasPorNoche + 1);

        foreach (Tarea tarea in tareasPosibles)
        {
            tarea.completada = false;
        }

        List<Tarea> tareasDisponibles = new List<Tarea>(tareasPosibles);
        List<Tarea> tareasSeleccionadas = new List<Tarea>();

        for (int i = 0; i < numTareas && tareasDisponibles.Count > 0; i++)
        {
            int indice = Random.Range(0, tareasDisponibles.Count);
            tareasSeleccionadas.Add(tareasDisponibles[indice]);
            tareasDisponibles.RemoveAt(indice);
        }

        if (tareasSeleccionadas.Count > 0)
        {
            ActivarTarea(tareasSeleccionadas[0]);
        }

        Debug.Log($"📋 Noche {noche}: {numTareas} tarea(s)");
    }

    void ActivarTarea(Tarea tarea)
    {
        tareaActual = tarea;
        tareaActiva = true;

        if (uiManager != null)
            uiManager.MostrarAvisoTarea(true, tareaActual.id);

        Debug.Log($"❗ Tarea activada: {tarea.nombre} en {tarea.escenaDestino}");

        // Si ya estamos en la escena correcta, activar interactuabilidad
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

        if (tareaActual.objetoEnEscena != null)
        {
            AbrirMinijuego interactuable = tareaActual.objetoEnEscena.GetComponent<AbrirMinijuego>();
            if (interactuable != null)
                interactuable.SetPuedeInteractuar(false);
        }

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
            gestion.CompletarTarea();

        // Buscar siguiente tarea
        Tarea siguienteTarea = null;
        foreach (Tarea t in tareasPosibles)
        {
            if (!t.completada && t != tareaActual)
            {
                siguienteTarea = t;
                break;
            }
        }

        if (siguienteTarea != null)
        {
            ActivarTarea(siguienteTarea);
        }
        else
        {
            tareaActiva = false;
            tareaActual = null;

            if (uiManager != null)
                uiManager.MostrarAvisoTarea(false, "");

            Debug.Log("✅ Todas las tareas completadas");
        }
    }
}