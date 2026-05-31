using UnityEngine;

public class AbrirMinijuego : MonoBehaviour
{
    public GameObject canvasMinijuego;
    public string ubicacionTarea;

    private Outline outlineComponent;
    private TareaManager tareaManager;
    private bool puedeInteractuar = false;
    private bool buscando = false;

    void Start()
    {
        outlineComponent = GetComponent<Outline>();
        if (outlineComponent == null)
            outlineComponent = gameObject.AddComponent<Outline>();
        outlineComponent.enabled = false;

        BuscarTareaManager();
    }

    void BuscarTareaManager()
    {
        tareaManager = FindObjectOfType<TareaManager>();
        if (tareaManager != null)
        {
            Debug.Log($"AbrirMinijuego en {gameObject.name}: TareaManager encontrado.");
            buscando = false;
        }
        else
        {
            Debug.LogWarning($"AbrirMinijuego en {gameObject.name}: No se encontró TareaManager, reintentando en 0.5s...");
            if (!buscando)
            {
                buscando = true;
                Invoke(nameof(BuscarTareaManager), 0.5f);
            }
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