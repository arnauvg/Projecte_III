using UnityEngine;
using System.Collections;

public class BotonPresionAnimacion : MonoBehaviour
{
    [Header("Partes del Botón")]
    public Transform parteMovil; // El objeto hijo que se moverá
    public Vector3 direccionMovimiento = new Vector3(0, 0, -0.000283f); // Dirección y distancia
    public float velocidadAnimacion = 15f; // Velocidad de la animación

    [Header("Sonido")]
    public AudioClip sonidoPresion;

    private Vector3 posicionOriginal;
    private bool animando = false;
    private AudioSource audioSource;

    void Start()
    {
        // Buscar automáticamente la parte móvil si no está asignada
        if (parteMovil == null)
        {
            if (transform.childCount > 0)
            {
                parteMovil = transform.GetChild(0);
                Debug.Log($"Parte móvil encontrada para {gameObject.name}: {parteMovil.name}");
            }
            else
            {
                Debug.LogError($"No se encontró parte móvil para el botón: {gameObject.name}");
            }
        }

        // Guardar posición original
        if (parteMovil != null)
        {
            posicionOriginal = parteMovil.localPosition;
            Debug.Log($"Posición original de {gameObject.name}: {posicionOriginal}");
        }

        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    public void Presionar()
    {
        if (animando) return;

        Debug.Log($"Botón {gameObject.name} presionado");

        // Reproducir sonido
        if (sonidoPresion != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoPresion);
        }

        StartCoroutine(AnimarPresion());
    }

    IEnumerator AnimarPresion()
    {
        animando = true;

        if (parteMovil != null)
        {
            Vector3 posicionPresionada = posicionOriginal + direccionMovimiento;

            // Bajar (moverse en la dirección indicada)
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * velocidadAnimacion;
                parteMovil.localPosition = Vector3.Lerp(posicionOriginal, posicionPresionada, t);
                yield return null;
            }

            // Asegurar posición final
            parteMovil.localPosition = posicionPresionada;

            // Pequeña pausa
            yield return new WaitForSeconds(0.05f);

            // Subir (volver)
            t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * velocidadAnimacion;
                parteMovil.localPosition = Vector3.Lerp(posicionPresionada, posicionOriginal, t);
                yield return null;
            }

            // Asegurar posición original
            parteMovil.localPosition = posicionOriginal;
        }

        animando = false;
    }
}