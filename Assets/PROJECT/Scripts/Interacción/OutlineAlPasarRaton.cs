using UnityEngine;

public class OutlineAlPasarRaton : MonoBehaviour
{
    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
        {
            outline.enabled = false;
        }
        else
        {
            Debug.LogError("Este objeto no tiene componente Outline: " + gameObject.name);
        }
    }

    void OnMouseEnter()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    void OnMouseExit()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}