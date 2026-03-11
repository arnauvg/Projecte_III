using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        
    }
    
}
