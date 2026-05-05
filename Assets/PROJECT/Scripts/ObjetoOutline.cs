using UnityEngine;

public class ObjetoOutline : MonoBehaviour
{
    private Outline outline;

    void Awake()
    {
        outline = GetComponentInChildren<Outline>();

        if (outline != null)
        {
            outline.enabled = false;
        }
        else
        {
            Debug.LogError("Falta el componente Outline en: " + gameObject.name);
        }
    }

    public void ActivarOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void DesactivarOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
