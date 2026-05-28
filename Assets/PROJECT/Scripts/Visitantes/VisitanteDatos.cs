using UnityEngine;

[CreateAssetMenu(fileName = "NuevoVisitante", menuName = "Juego/Visitante")]
public class VisitanteDatos : ScriptableObject
{
    public string nombreVisitante;

    [Header("Sprites")]
    public Sprite spriteNormal;
    public Sprite spriteRevelado;

    [Header("Tipo")]
    public bool esDoble;
}