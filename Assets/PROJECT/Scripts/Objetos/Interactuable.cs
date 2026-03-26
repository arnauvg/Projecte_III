using UnityEngine;

public abstract class Interactuable : MonoBehaviour
{
    // Método para recoger el objeto (retorna true si se pudo recoger)
    public abstract bool Recoger();

    // Método para soltar el objeto
    public abstract void Soltar();
}