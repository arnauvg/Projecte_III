using UnityEngine;
using UnityEngine.EventSystems;

public class DebugEventSystem : MonoBehaviour
{
    void Start()
    {
        EventSystem es = FindFirstObjectByType<EventSystem>();
        Debug.Log($"EventSystem encontrado: {es != null}");

        if (es != null)
        {
            Debug.Log($"Nombre del EventSystem: {es.gameObject.name}");
            Debug.Log($"StandaloneInputModule: {es.GetComponent<StandaloneInputModule>() != null}");
        }
    }
}