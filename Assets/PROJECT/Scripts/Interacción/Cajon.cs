using UnityEngine;

public class Cajon : MonoBehaviour
{
    public Animator animator;
    public string openParam = "isOpen";
    private bool isOpen = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        isOpen = !isOpen;
        animator.SetBool(openParam, isOpen);
    }

    // 👇 NUEVA FUNCIÓN: Devuelve si el cajón está abierto
    public bool EstaAbierto()
    {
        return isOpen;
    }
}