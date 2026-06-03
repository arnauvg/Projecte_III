using UnityEngine;

public class InitialLoader : MonoBehaviour
{
    [Header("Prefab del PersistentGameManager")]
    public GameObject persistentGameManagerPrefab;

    void Awake()
    {
        // Verificar si ya existe un PersistentGameManager
        if (FindFirstObjectByType<PersistentGameManager>() == null)
        {
            if (persistentGameManagerPrefab != null)
            {
                Instantiate(persistentGameManagerPrefab);
                Debug.Log("InitialLoader: PersistentGameManager instanciado");
            }
            else
            {
                Debug.LogError("InitialLoader: No hay prefab asignado");
            }
        }
    }
}