using UnityEngine;

public class AbrirMinijuegoAgua : MonoBehaviour
{
    public MinijuegoAguaBendita minijuegoAgua;

    void OnMouseDown()
    {
        minijuegoAgua.AbrirMinijuego();
    }
}