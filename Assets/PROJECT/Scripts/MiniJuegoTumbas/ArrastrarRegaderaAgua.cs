using UnityEngine;
using UnityEngine.EventSystems;

public class ArrastrarRegaderaAgua : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GestorMinijuegoAgua gestor;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 posicionInicial;
    private bool puedoArrastrar = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        posicionInicial = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gestor == null) return;

        if (!gestor.PuedeMoverRegadera())
        {
            Debug.Log("La regadera todavía no está llena");
            puedoArrastrar = false;
            return;
        }

        puedoArrastrar = true;
        gestor.EmpezarMoverRegadera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!puedoArrastrar) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!puedoArrastrar) return;

        gestor.SoltarRegadera(rectTransform);

        rectTransform.anchoredPosition = posicionInicial;
        puedoArrastrar = false;
    }
}