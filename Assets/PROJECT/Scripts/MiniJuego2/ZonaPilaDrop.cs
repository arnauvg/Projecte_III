using UnityEngine;
using UnityEngine.EventSystems;

public class ZonaPilaDrop : MonoBehaviour, IDropHandler
{
    public MinijuegoAguaBendita minijuegoAgua;

    public void OnDrop(PointerEventData eventData)
    {
        ObjetoArrastrableUI objeto = eventData.pointerDrag.GetComponent<ObjetoArrastrableUI>();

        if (objeto == null) return;

        if (objeto.tipoObjeto == "Trapo")
        {
            minijuegoAgua.UsarTrapo();
        }
        else if (objeto.tipoObjeto == "Agua")
        {
            minijuegoAgua.UsarAguaBendita();
        }
    }
}