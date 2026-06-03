using UnityEngine;

public class AbrirMinijuego : MonoBehaviour
{
    public GameObject canvasMinijuego;
    public string ubicacionTarea;

    private Outline outlineComponent;
    private TareaManager tareaManager;
    private bool puedeInteractuar = false;
    private MouseLook360 mouseLook;

    void Start()
    {
        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
            outlineComponent = gameObject.AddComponent<Outline>();
        outlineComponent.enabled = false;

        tareaManager = FindFirstObjectByType<TareaManager>();
        if (tareaManager == null)
            Invoke("BuscarTareaManager", 0.5f);

        mouseLook = FindFirstObjectByType<MouseLook360>();
    }

    void BuscarTareaManager()
    {
        tareaManager = FindFirstObjectByType<TareaManager>();
    }

    void Update()
    {
        if (tareaManager != null)
            puedeInteractuar = tareaManager.PuedeInteractuarConObjeto(gameObject);
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

            if (tareaManager != null)
                tareaManager.BloquearMapaPorTarea();

            // 🔥 Desactivar el control de la cámara
            if (mouseLook != null) mouseLook.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public void SetPuedeInteractuar(bool puede)
    {
        puedeInteractuar = puede;
        if (!puede && outlineComponent != null)
            outlineComponent.enabled = false;
    }
}