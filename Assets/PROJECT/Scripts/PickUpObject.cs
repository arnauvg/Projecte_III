using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public float pickDistance = 4f;
    public float moveSpeed = 10f;

    Rigidbody heldObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (heldObject == null)
                TryPickUp();
            else
                Drop();
        if (Physics.Raycast(ray, out RaycastHit hit, 4f))
        {
            if (hit.rigidbody != null)
            {
                Debug.Log("Objeto detectado");
            }
        }
        }

        if (heldObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Camera.main.transform.forward * -1,
                                    Camera.main.transform.position + Camera.main.transform.forward * 1f);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 targetPos = ray.GetPoint(distance);
                Vector3 dir = targetPos - heldObject.position;
                heldObject.linearVelocity = dir * moveSpeed;
            }
        }
    }

    void TryPickUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, pickDistance))
        {
            if (hit.rigidbody != null)
            {
                heldObject = hit.rigidbody;
                heldObject.useGravity = false;
            }
        }
    }

    void Drop()
    {
        heldObject.useGravity = true;
        heldObject = null;
    }
}