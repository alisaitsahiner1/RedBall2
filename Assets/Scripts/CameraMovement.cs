using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target; // Takip edilecek karakter (top)
    public float smoothSpeed = 0.1f; // Kameranın yumuşak hareket etme hızı
    public Vector3 offset; // Kameranın karaktere göre konumu

    void LateUpdate()
    {
        if (target == null) return; // Eğer hedef yoksa çık

        // Kameranın X ekseninde topu takip etmesini sağla (Y ekseni sabit kalacak)
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        
        // Yumuşak geçiş
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
