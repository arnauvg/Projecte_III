using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public enum TipoZona
    {
        Papelera,
        SlotVelaNueva
    }

    public TipoZona tipoZona;

    public string itemIDCorrecto;   // Solo para slots de velas nuevas
    public MinijuegoVelas manager;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem item = eventData.pointerDrag.GetComponent<DraggableItem>();

        if (item == null) return;

        // Caso 1: Papelera acepta velas viejas
        if (tipoZona == TipoZona.Papelera && item.esVelaVieja)
        {
            item.DesactivarObjeto();
            manager.VelaViejaEliminada();
            return;
        }

        // Caso 2: Slot acepta su vela nueva correcta
        if (tipoZona == TipoZona.SlotVelaNueva && item.esVelaNueva)
        {
            if (item.itemID == itemIDCorrecto)
            {
                item.ColocarEnDestino(transform);
                manager.VelaNuevaColocada();
            }
            else
            {
                item.VolverAPosicionInicial();
            }
        }
        else
        {
            item.VolverAPosicionInicial();
        }
    }
}