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
    public UIManager uiManager;

    public static System.Action OnPrimeraTareaActivada;

    private List<Tarea> tareasActivas = new List<Tarea>();
    private int nocheActual = 1;
    private bool generacionTareasPausada = false;
    private bool primeraTareaYaNotificada = false;
    private int tareasGeneradas = 0;
    private int tareasPendientesGenerar = 2;
    private bool nochePreparada = false;
    private int ultimaHoraGeneracion = -1;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        CronometroNoche.OnHoraCambiada += VerificarGenerarTarea;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        CronometroNoche.OnHoraCambiada -= VerificarGenerarTarea;
    }

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

                    // Verificar si esta tarea está activa y no completada
                    bool estaActiva = false;
                    foreach (Tarea activa in tareasActivas)
                    {
                        if (activa.id == tarea.id && !activa.completada)
                        {
                            estaActiva = true;
                            break;
                        }
                    }

                    if (estaActiva)
                    {
                        ActivarInteractuabilidadEnObjeto(encontrado);
                        Debug.Log($"✅ Interactuabilidad activada para: {encontrado.name}");
                    }
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

    void DesactivarInteractuabilidadEnObjeto(GameObject obj)
    {
        AbrirMinijuego interactuable = obj.GetComponent<AbrirMinijuego>();
        if (interactuable != null)
            interactuable.SetPuedeInteractuar(false);
    }

    void VerificarGenerarTarea(int horas, int minutos)
    {
        if (generacionTareasPausada) return;
        if (!nochePreparada) return;
        if (tareasGeneradas >= tareasPendientesGenerar) return;
        if (ultimaHoraGeneracion == horas) return;

        Debug.Log($"📋 Verificando - Hora: {horas}:{minutos:00}, Generadas: {tareasGeneradas}/{tareasPendientesGenerar}");

        ultimaHoraGeneracion = horas;
        GenerarUnaTarea();
    }

    void GenerarUnaTarea()
    {
        if (tareasGeneradas >= tareasPendientesGenerar) return;

        if (tareasPosibles.Count == 0)
        {
            Debug.Log("No hay tareas disponibles");
            return;
        }

        // Seleccionar una tarea aleatoria que no esté ya activa
        List<Tarea> disponibles = new List<Tarea>();
        foreach (Tarea t in tareasPosibles)
        {
            bool yaActiva = false;
            foreach (Tarea activa in tareasActivas)
            {
                if (activa.id == t.id)
                {
                    yaActiva = true;
                    break;
                }
            }
            if (!yaActiva) disponibles.Add(t);
        }

        if (disponibles.Count == 0)
        {
            Debug.Log("Todas las tareas ya están activas");
            return;
        }

        int idx = Random.Range(0, disponibles.Count);
        Tarea tareaSeleccionada = disponibles[idx];

        tareaSeleccionada.completada = false;
        ActivarTarea(tareaSeleccionada);
        tareasGeneradas++;
        Debug.Log($"📋 Tarea {tareasGeneradas}/{tareasPendientesGenerar} generada: {tareaSeleccionada.nombre} a las {ultimaHoraGeneracion}:00");
    }

    void ActivarTarea(Tarea tarea)
    {
        tareasActivas.Add(tarea);

        if (uiManager != null)
            uiManager.AgregarTareaActiva(tarea.id);

        if (tarea.objetoEnEscena != null)
        {
            ActivarInteractuabilidadEnObjeto(tarea.objetoEnEscena);
        }
        else
        {
            string escenaActual = SceneManager.GetActiveScene().name;
            if (tarea.escenaDestino == escenaActual)
            {
                GameObject encontrado = GameObject.FindGameObjectWithTag($"Tarea{tarea.id}");
                if (encontrado != null)
                {
                    tarea.objetoEnEscena = encontrado;
                    ActivarInteractuabilidadEnObjeto(encontrado);
                    Debug.Log($"🔍 Objeto {encontrado.name} encontrado y activado");
                }
            }
        }

        if (!primeraTareaYaNotificada && nocheActual == 1 && tareasGeneradas == 1)
        {
            primeraTareaYaNotificada = true;
            Debug.Log("📞 Primera tarea activada - Llamando al jefe...");
            OnPrimeraTareaActivada?.Invoke();
        }

        Debug.Log($"❗ Tarea activada: {tarea.nombre} en {tarea.escenaDestino}");
    }

    public bool PuedeInteractuarConObjeto(GameObject objeto)
    {
        foreach (Tarea tarea in tareasActivas)
        {
            if (!tarea.completada && tarea.objetoEnEscena == objeto)
            {
                string escenaActual = SceneManager.GetActiveScene().name;
                if (tarea.escenaDestino == escenaActual)
                    return true;
            }
        }
        return false;
    }

    public void CompletarTareaPorObjeto(GameObject objeto)
    {
        Debug.Log($"CompletarTareaPorObjeto llamado para: {objeto.name}");

        for (int i = 0; i < tareasActivas.Count; i++)
        {
            Tarea tarea = tareasActivas[i];
            if (!tarea.completada && tarea.objetoEnEscena == objeto)
            {
                CompletarTareaEspecifica(tarea);
                return;
            }
        }
        Debug.LogWarning($"No se encontró tarea activa para el objeto {objeto.name}");
    }

    public void CompletarTareaActual()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        Debug.Log($"CompletarTareaActual llamado en escena: {escenaActual}");

        for (int i = 0; i < tareasActivas.Count; i++)
        {
            Tarea tarea = tareasActivas[i];
            if (!tarea.completada && tarea.escenaDestino == escenaActual)
            {
                CompletarTareaEspecifica(tarea);
                return;
            }
        }
        Debug.LogWarning("No se encontró tarea activa en la escena actual");
    }

    void CompletarTareaEspecifica(Tarea tarea)
    {
        if (tarea.completada) return;

        tarea.completada = true;
        Debug.Log($"✅ Tarea completada: {tarea.nombre}");

        // Eliminar de la lista de activas
        tareasActivas.Remove(tarea);

        // Eliminar de la UI
        if (uiManager != null)
            uiManager.EliminarTareaActiva(tarea.id);

        // Desactivar interactuabilidad
        if (tarea.objetoEnEscena != null)
        {
            DesactivarInteractuabilidadEnObjeto(tarea.objetoEnEscena);
            Debug.Log($"🔒 Interactuabilidad desactivada para: {tarea.objetoEnEscena.name}");
        }

        GestionNoches gestion = FindFirstObjectByType<GestionNoches>();
        if (gestion != null)
            gestion.CompletarTarea();

        Debug.Log($"Quedan {tareasActivas.Count} tareas activas");
    }

    public void IniciarNoche(int noche)
    {
        nocheActual = noche;
        tareasGeneradas = 0;
        tareasActivas.Clear();
        nochePreparada = true;
        ultimaHoraGeneracion = -1;

        foreach (Tarea t in tareasPosibles)
        {
            t.completada = false;
            if (t.objetoEnEscena != null)
                DesactivarInteractuabilidadEnObjeto(t.objetoEnEscena);
        }

        Debug.Log($"📋 Noche {noche} preparada. Se generarán {tareasPendientesGenerar} tareas (una por hora entre 01:00 y 05:00)");
    }

    public void PausarGeneracionTareas(bool pausar) => generacionTareasPausada = pausar;

    public void ReiniciarParaNuevaPartida()
    {
        primeraTareaYaNotificada = false;
        tareasActivas.Clear();
        tareasGeneradas = 0;
        nochePreparada = false;
        ultimaHoraGeneracion = -1;
    }
}