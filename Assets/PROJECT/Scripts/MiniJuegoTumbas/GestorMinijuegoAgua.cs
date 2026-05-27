using UnityEngine;

public class GestorMinijuegoAgua : MonoBehaviour
{
    [Header("Tumba")]
    public GameObject tumbaNormal;
    public GameObject tumbaFlores;

    [Header("Grifo")]
    public GameObject grifoNormal;
    public GameObject grifoAbierto;

    [Header("Regadera")]
    public GameObject regaderaNormal;
    public GameObject regaderaLlena;
    public GameObject regaderaAguaSale;

    // Referencia para detección por posición (opcional, solo si usas UI)
    [Header("Opcional - UI")]
    public RectTransform tumbaRectTransform;  // Asigna aquí el RectTransform de la tumba (si es UI)

    private bool tieneAgua = false;
    private bool completado = false;

    void Start()
    {
        ReiniciarMinijuego();
    }

    public void ClickGrifo()
    {
        if (completado) return;

        Debug.Log("Has clicado el grifo");

        tieneAgua = true;

        grifoNormal.SetActive(false);
        grifoAbierto.SetActive(true);

        regaderaNormal.SetActive(false);
        regaderaLlena.SetActive(true);
        regaderaAguaSale.SetActive(false);
    }

    public void ClickRegadera()
    {
        if (completado) return;

        if (!tieneAgua)
        {
            Debug.Log("Primero tienes que abrir el grifo");
            return;
        }

        Debug.Log("Has cogido la regadera llena");

        regaderaNormal.SetActive(false);
        regaderaLlena.SetActive(false);
        regaderaAguaSale.SetActive(true);
    }

    public void ClickTumba()
    {
        if (completado) return;

        if (!tieneAgua)
        {
            Debug.Log("Primero tienes que llenar la regadera");
            return;
        }

        Debug.Log("Has regado la tumba");

        completado = true;

        tumbaNormal.SetActive(false);
        tumbaFlores.SetActive(true);

        regaderaNormal.SetActive(false);
        regaderaLlena.SetActive(false);
        regaderaAguaSale.SetActive(true);

        grifoAbierto.SetActive(false);
        grifoNormal.SetActive(true);
    }

    // NUEVO MÉTODO: Se llama cuando se suelta la regadera
    public void ComprobarSuelta(RectTransform regaderaRect)
    {
        if (completado) return;
        if (!tieneAgua)
        {
            Debug.Log("La regadera no tiene agua aún.");
            return;
        }

        // ---------- OPCIÓN A (fácil): Riega siempre al soltar ----------
        ClickTumba();

        // ---------- OPCIÓN B (detección por posición): Solo riega si la regadera está cerca de la tumba ----------
        /*
        if (tumbaRectTransform != null)
        {
            float distancia = Vector2.Distance(regaderaRect.anchoredPosition, tumbaRectTransform.anchoredPosition);
            if (distancia < 100f)  // Ajusta el umbral según tu interfaz
            {
                ClickTumba();
            }
            else
            {
                Debug.Log("Debes soltar la regadera sobre la tumba");
            }
        }
        else
        {
            Debug.LogWarning("No asignaste el RectTransform de la tumba en el GestorMinijuegoAgua");
        }
        */
    }

    public void ReiniciarMinijuego()
    {
        tieneAgua = false;
        completado = false;

        tumbaNormal.SetActive(true);
        tumbaFlores.SetActive(false);

        grifoNormal.SetActive(true);
        grifoAbierto.SetActive(false);

        regaderaNormal.SetActive(true);
        regaderaLlena.SetActive(false);
        regaderaAguaSale.SetActive(false);
    }
}