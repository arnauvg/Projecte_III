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

    private bool grifoAbiertoEstado = false;
    private bool regaderaEstaLlena = false;
    private bool tareaCompletada = false;
    private bool llenando = false;

    void Start()
    {
        ReiniciarMinijuego();
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
        Debug.Log("Tumba regada. Tarea completada.");

        tareaCompletada = true;

        tumbaNormal.SetActive(false);
        tumbaFlores.SetActive(true);

        // La regadera vuelve a su estado normal/vacía
        regaderaNormal.SetActive(true);
        regaderaLlena.SetActive(false);
        regaderaAguaSale.SetActive(false);

        // El grifo queda cerrado
        grifoNormal.SetActive(true);
        grifoAbierto.SetActive(false);
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