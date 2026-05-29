using System.Collections;
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

    [Header("Zona")]
    public RectTransform zonaTumba;

    [Header("Tiempo")]
    public float tiempoLlenado = 2f;

    [Header("Cierre automático")]
    public CerrarMinijuego cerrarMinijuego;

    private bool grifoAbiertoEstado = false;
    private bool regaderaEstaLlena = false;
    private bool tareaCompletada = false;
    private bool llenando = false;

    void Start()
    {
        ReiniciarMinijuego();

        if (cerrarMinijuego == null)
            cerrarMinijuego = FindFirstObjectByType<CerrarMinijuego>();

        Debug.Log("GestorMinijuegoAgua iniciado. cerrarMinijuego = " + (cerrarMinijuego != null ? "ASIGNADO" : "NULL"));
    }

    public void ClickGrifo()
    {
        if (tareaCompletada) return;

        if (!grifoAbiertoEstado)
        {
            AbrirGrifo();
        }
        else
        {
            CerrarGrifo();
        }
    }

    void AbrirGrifo()
    {
        Debug.Log("Grifo abierto");

        grifoAbiertoEstado = true;

        grifoNormal.SetActive(false);
        grifoAbierto.SetActive(true);

        if (!regaderaEstaLlena && !llenando)
        {
            StartCoroutine(LlenarRegaderaDespuesDeTiempo());
        }
    }

    IEnumerator LlenarRegaderaDespuesDeTiempo()
    {
        llenando = true;

        yield return new WaitForSecondsRealtime(tiempoLlenado);

        regaderaEstaLlena = true;
        llenando = false;

        regaderaNormal.SetActive(false);
        regaderaLlena.SetActive(true);
        regaderaAguaSale.SetActive(false);

        Debug.Log("Regadera llena");
    }

    void CerrarGrifo()
    {
        Debug.Log("Grifo cerrado");

        grifoAbiertoEstado = false;

        grifoNormal.SetActive(true);
        grifoAbierto.SetActive(false);
    }

    public bool PuedeMoverRegadera()
    {
        return regaderaEstaLlena && !tareaCompletada;
    }

    public void EmpezarMoverRegadera()
    {
        if (!PuedeMoverRegadera()) return;

        Debug.Log("Has cogido la regadera llena");

        regaderaNormal.SetActive(false);
        regaderaLlena.SetActive(false);
        regaderaAguaSale.SetActive(true);
    }

    public void SoltarRegadera(RectTransform regadera)
    {
        if (!PuedeMoverRegadera()) return;

        if (EstaEncimaDeZona(regadera, zonaTumba))
        {
            CompletarTarea();
        }
        else
        {
            Debug.Log("No has soltado la regadera sobre la tumba");

            regaderaNormal.SetActive(false);
            regaderaLlena.SetActive(true);
            regaderaAguaSale.SetActive(false);
        }
    }

    bool EstaEncimaDeZona(RectTransform objeto, RectTransform zona)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            zona,
            objeto.position,
            null
        );
    }

    void CompletarTarea()
    {
        if (tareaCompletada) return;

        Debug.Log("Tumba regada. Mostrando flores...");

        tareaCompletada = true;

        // Mostrar la tumba con flores
        tumbaNormal.SetActive(false);
        tumbaFlores.SetActive(true);

        // Resetear regadera
        regaderaNormal.SetActive(true);
        regaderaLlena.SetActive(false);
        regaderaAguaSale.SetActive(false);

        // Resetear grifo
        grifoNormal.SetActive(true);
        grifoAbierto.SetActive(false);

        grifoAbiertoEstado = false;
        regaderaEstaLlena = false;

        // Cerrar el minijuego después de 1 segundo (para que se vean las flores)
        StartCoroutine(CerrarConDelay());
    }

    IEnumerator CerrarConDelay()
    {
        Debug.Log("Esperando 1 segundo antes de cerrar...");
        yield return new WaitForSecondsRealtime(1f);

        if (cerrarMinijuego != null)
        {
            Debug.Log("Cerrando minijuego...");
            cerrarMinijuego.CompletarYCerrar();
        }
        else
        {
            Debug.LogError("cerrarMinijuego es NULL");
        }
    }

    public void ReiniciarMinijuego()
    {
        grifoAbiertoEstado = false;
        regaderaEstaLlena = false;
        tareaCompletada = false;
        llenando = false;

        tumbaNormal.SetActive(true);
        tumbaFlores.SetActive(false);

        grifoNormal.SetActive(true);
        grifoAbierto.SetActive(false);

        regaderaNormal.SetActive(true);
        regaderaLlena.SetActive(false);
        regaderaAguaSale.SetActive(false);
    }
}