using UnityEngine;

public class MinijuegoAguaBendita : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject canvasMinijuego;

    [Header("Estados visuales de la pila")]
    public GameObject pilaMuySucia;
    public GameObject pilaAguaSucia;
    public GameObject pilaVacia;
    public GameObject pilaLlenaAguaLimpia;

    private int estadoPila = 0;
    private bool minijuegoCompletado = false;

    void Start()
    {
        if (canvasMinijuego != null)
            canvasMinijuego.SetActive(false);

        MostrarEstadoInicial();
    }

    void MostrarEstadoInicial()
    {
        estadoPila = 0;
        minijuegoCompletado = false;

        pilaMuySucia.SetActive(true);
        pilaAguaSucia.SetActive(false);
        pilaVacia.SetActive(false);
        pilaLlenaAguaLimpia.SetActive(false);
    }

    public void AbrirMinijuego()
    {
        canvasMinijuego.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarMinijuego()
    {
        canvasMinijuego.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UsarTrapo()
    {
        if (minijuegoCompletado) return;

        if (estadoPila == 0)
        {
            // De pila muy sucia a pila con agua sucia
            pilaMuySucia.SetActive(false);
            pilaAguaSucia.SetActive(true);
            pilaVacia.SetActive(false);
            pilaLlenaAguaLimpia.SetActive(false);

            estadoPila = 1;

            Debug.Log("Primer paso: has limpiado la suciedad exterior.");
        }
        else if (estadoPila == 1)
        {
            // De pila con agua sucia a pila vacía
            pilaMuySucia.SetActive(false);
            pilaAguaSucia.SetActive(false);
            pilaVacia.SetActive(true);
            pilaLlenaAguaLimpia.SetActive(false);

            estadoPila = 2;

            Debug.Log("Segundo paso: has quitado el agua sucia. Ahora falta rellenar.");
        }
        else if (estadoPila == 2)
        {
            Debug.Log("La pila ya está limpia. Ahora usa la botella de agua bendita.");
        }
    }

    public void UsarAguaBendita()
    {
        if (minijuegoCompletado) return;

        if (estadoPila < 2)
        {
            Debug.Log("Primero tienes que limpiar la pila con el trapo.");
            return;
        }

        if (estadoPila == 2)
        {
            // De pila vacía a pila llena con agua limpia
            pilaMuySucia.SetActive(false);
            pilaAguaSucia.SetActive(false);
            pilaVacia.SetActive(false);
            pilaLlenaAguaLimpia.SetActive(true);

            estadoPila = 3;
            minijuegoCompletado = true;

            Debug.Log("Minijuego completado: pila limpia y llena de agua bendita.");
        }
    }
}