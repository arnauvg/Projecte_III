using UnityEngine;

public class AbrirMinijuego : MonoBehaviour
{
    public GameObject canvasMinijuego;
    public string ubicacionTarea;

    private Outline outlineComponent;
    private TareaManager tareaManager;
    private bool puedeInteractuar = false;

    void Start()
    {
        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
            outlineComponent = gameObject.AddComponent<Outline>();
        outlineComponent.enabled = false;

        // Buscar TareaManager
        tareaManager = FindFirstObjectByType<TareaManager>();

        if (tareaManager == null)
        {
            Debug.LogWarning($"No se encontró TareaManager, buscando de nuevo...");
            Invoke("BuscarTareaManager", 0.5f);
        }
    }

    void BuscarTareaManager()
    {
        tareaManager = FindFirstObjectByType<TareaManager>();
        if (tareaManager != null)
        {
            Debug.Log($"TareaManager encontrado en {gameObject.name}");
        }
    }

    void Update()
    {
        if (tareaManager != null)
        {
            puedeInteractuar = tareaManager.PuedeInteractuarConObjeto(gameObject);
        }
    }

    void OnMouseEnter()
    {
        if (puedeInteractuar && outlineComponent != null)
            outlineComponent.enabled = true;
    }

    void OnMouseExit()
    {
        if (outlineComponent != null)
            outlineComponent.enabled = false;
    }

    void OnMouseDown()
    {
        if (puedeInteractuar && canvasMinijuego != null)
        {
            canvasMinijuego.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public void SetPuedeInteractuar(bool puede)
    {
        puedeInteractuar = puede;
    }
}