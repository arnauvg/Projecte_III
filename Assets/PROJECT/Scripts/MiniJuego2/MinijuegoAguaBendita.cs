using UnityEngine;

public class MinijuegoAguaBendita : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject canvasMinijuego;

    [Header("Estados de la pila")]
    public GameObject pilaSucia1;
    public GameObject pilaSucia2;
    public GameObject pilaLimpiaSinAgua;
    public GameObject pilaLimpiaConAgua;

    private int nivelLimpieza = 0;
    private bool tieneAgua = false;
    private bool minijuegoCompletado = false;

    void Start()
    {
        if (canvasMinijuego != null)
            canvasMinijuego.SetActive(false);

        MostrarEstadoInicial();
    }

    void MostrarEstadoInicial()
    {
        pilaSucia1.SetActive(true);
        pilaSucia2.SetActive(false);
        pilaLimpiaSinAgua.SetActive(false);
        pilaLimpiaConAgua.SetActive(false);

        nivelLimpieza = 0;
        tieneAgua = false;
        minijuegoCompletado = false;
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
        if (tieneAgua) return;

        nivelLimpieza++;

        if (nivelLimpieza == 1)
        {
            pilaSucia1.SetActive(false);
            pilaSucia2.SetActive(true);
            pilaLimpiaSinAgua.SetActive(false);
            pilaLimpiaConAgua.SetActive(false);

            Debug.Log("Primer paso de limpieza completado.");
        }
        else if (nivelLimpieza == 2)
        {
            pilaSucia1.SetActive(false);
            pilaSucia2.SetActive(false);
            pilaLimpiaSinAgua.SetActive(true);
            pilaLimpiaConAgua.SetActive(false);

            Debug.Log("Pila limpia. Ahora puedes rellenarla con agua bendita.");
        }
        else
        {
            nivelLimpieza = 2;
        }
    }

    public void UsarAguaBendita()
    {
        if (minijuegoCompletado) return;

        if (nivelLimpieza < 2)
        {
            Debug.Log("Primero tienes que limpiar completamente la pila.");
            return;
        }

        pilaSucia1.SetActive(false);
        pilaSucia2.SetActive(false);
        pilaLimpiaSinAgua.SetActive(false);
        pilaLimpiaConAgua.SetActive(true);

        tieneAgua = true;
        minijuegoCompletado = true;

        Debug.Log("Minijuego completado: pila limpia y llena de agua bendita.");
    }
}