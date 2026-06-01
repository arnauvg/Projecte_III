using UnityEngine;

public enum TipoRevelador
{
    Ninguno,
    Ajo,
    Linterna,
    Cruz,
    Otro
}

[CreateAssetMenu(fileName = "NuevoVisitante", menuName = "Juego/Visitante")]
public class VisitanteDatos : ScriptableObject
{
    public string nombreVisitante;

    [Header("Sprites")]
    public Sprite spriteNormal;
    public Sprite spriteRevelado;

    [Header("Tipo")]
    public bool esDoble;

    [Header("Revelación")]
    public TipoRevelador reveladorNecesario = TipoRevelador.Ninguno;

    [Header("Diálogo")]
    [TextArea(2, 4)]
    public string dialogoBienvenida;  // ← NUEVO: texto que dice al entrar
}