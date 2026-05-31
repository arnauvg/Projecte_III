using UnityEngine;
using System.Collections;

public class MinijuegoAguaBendita : MonoBehaviour
{
    [Header("Referencias UI")]
    public CerrarMinijuego cerrarMinijuego;   // arrastra el botón Salir

    [Header("Estados visuales de la pila")]
    public GameObject pilaMuySucia;
    public GameObject pilaAguaSucia;
    public GameObject pilaVacia;
    public GameObject pilaLlenaAguaLimpia;

    private int estadoPila = 0;      // 0=muy sucia, 1=agua sucia, 2=vacia, 3=llena
    private bool completado = false;

    void Start()
    {
        MostrarEstadoInicial();
        if (cerrarMinijuego == null)
            cerrarMinijuego = FindObjectOfType<CerrarMinijuego>();
    }

    void MostrarEstadoInicial()
    {
        estadoPila = 0;
        pilaMuySucia.SetActive(true);
        pilaAguaSucia.SetActive(false);
        pilaVacia.SetActive(false);
        pilaLlenaAguaLimpia.SetActive(false);
    }

    // Método llamado desde ZonaPilaDrop cuando usan el trapo
    public void UsarTrapo()
    {
        if (completado) return;

        if (estadoPila == 0)
        {
            // De muy sucia a agua sucia
            pilaMuySucia.SetActive(false);
            pilaAguaSucia.SetActive(true);
            estadoPila = 1;
            Debug.Log("Primer paso: has limpiado la suciedad exterior.");
        }
        else if (estadoPila == 1)
        {
            // De agua sucia a vacía
            pilaAguaSucia.SetActive(false);
            pilaVacia.SetActive(true);
            estadoPila = 2;
            Debug.Log("Segundo paso: has quitado el agua sucia. Ahora falta rellenar.");
        }
        else
        {
            Debug.Log("No puedes usar el trapo ahora.");
        }
    }

    // Método llamado desde ZonaPilaDrop cuando usan el agua bendita
    public void UsarAguaBendita()
    {
        if (completado) return;

        if (estadoPila == 2)
        {
            // De vacía a llena
            pilaVacia.SetActive(false);
            pilaLlenaAguaLimpia.SetActive(true);
            estadoPila = 3;
            Completar();
        }
        else
        {
            Debug.Log("Primero limpia la pila con el trapo hasta dejarla vacía.");
        }
    }

    void Completar()
    {
        if (completado) return;
        completado = true;

        Debug.Log("Minijuego de agua bendita completado.");

        // Notificar a GestionNoches
        GestionNoches gestion = FindObjectOfType<GestionNoches>();
        if (gestion != null)
            gestion.CompletarTarea();

        // Cerrar con delay y sonido
        if (cerrarMinijuego != null)
            StartCoroutine(CerrarConDelay());
    }

    IEnumerator CerrarConDelay()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        cerrarMinijuego.CompletarYCerrar();
    }

    // Opcional: reiniciar el minijuego si se reutiliza
    public void Reiniciar()
    {
        completado = false;
        MostrarEstadoInicial();
    }
}