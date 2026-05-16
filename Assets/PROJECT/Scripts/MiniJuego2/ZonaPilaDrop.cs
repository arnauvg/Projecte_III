using UnityEngine;
using UnityEngine.EventSystems;

public class ZonaPilaDrop : MonoBehaviour, IDropHandler
{
    public MinijuegoAguaBendita minijuegoAgua;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("He soltado algo en ZonaPila");

        ObjetoArrastrableUI objeto = eventData.pointerDrag.GetComponent<ObjetoArrastrableUI>();

        if (objeto == null)
        {
            Debug.Log("El objeto soltado no tiene ObjetoArrastrableUI");
            return;
        }

        Debug.Log("Objeto soltado: " + objeto.tipoObjeto);

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