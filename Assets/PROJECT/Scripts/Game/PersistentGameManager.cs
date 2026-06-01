using UnityEngine;

public class PersistentGameManager : MonoBehaviour
{
    public static PersistentGameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("PersistentGameManager: Persistente");
    }
}