using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemID;                 // Ej: "Vieja", "Nueva1", "Nueva2"...
    public bool esVelaVieja = false;
    public bool esVelaNueva = false;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 posicionInicial;
    private Transform padreInicial;

    public bool colocadaCorrectamente = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (colocadaCorrectamente) return;

        posicionInicial = rectTransform.anchoredPosition;
        padreInicial = transform.parent;

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (colocadaCorrectamente) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (colocadaCorrectamente) return;

        canvasGroup.blocksRaycasts = true;
    }

    public void VolverAPosicionInicial()
    {
        transform.SetParent(padreInicial);
        rectTransform.anchoredPosition = posicionInicial;
    }

    public void ColocarEnDestino(Transform destino)
    {
        transform.SetParent(destino);
        rectTransform.anchoredPosition = Vector2.zero;
        colocadaCorrectamente = true;
    }

    public void DesactivarObjeto()
    {
        gameObject.SetActive(false);
    }
}