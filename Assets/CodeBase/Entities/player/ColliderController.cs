using UnityEngine;

public class ColliderController : MonoBehaviour
{
    public BoxCollider2D collider;

    void LateUpdate()
    {
        collider.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
