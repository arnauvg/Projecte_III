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

        // Desactivar outline al inicio
        outlineComponent.enabled = false;

        tareaManager = FindObjectOfType<TareaManager>();
        if (tareaManager == null)
            Invoke("BuscarTareaManager", 0.5f);
    }

    void BuscarTareaManager()
    {
        tareaManager = FindObjectOfType<TareaManager>();
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
        // Solo mostrar outline si se puede interactuar
        if (puedeInteractuar && outlineComponent != null)
            outlineComponent.enabled = true;
    }

    void OnMouseExit()
    {
        // Ocultar outline siempre al salir
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
        // Si no puede interactuar, asegurar que el outline se oculta
        if (!puedeInteractuar && outlineComponent != null)
            outlineComponent.enabled = false;
    }
}