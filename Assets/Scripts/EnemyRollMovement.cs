using UnityEngine;
using System.Collections;

public class EnemyRollMovement : MonoBehaviour
{
    private bool shouldFlipDirection = false;
    
    public Vector2 moveDirection = Vector2.right; // Başlangıçta sağa gidiyor
    private bool isMoving = false;

    void Update()
    {
        if (!isMoving)
        {
            StartCoroutine(RollStep());
        }
    }

    IEnumerator RollStep()
    {
        isMoving = true;

        float angle = 90f;
        float duration = 0.5f;
        float time = 0f;

        // Dönülecek merkez noktayı hesapla
        Vector3 pivot = GetPivotPoint();

        // Her adımda rotate et
        while (time < duration)
        {
            float step = (angle / duration) * Time.deltaTime;
            transform.RotateAround(pivot, Vector3.forward, -step * Mathf.Sign(moveDirection.x));
            time += Time.deltaTime;
            yield return null;
        }

        //rotation hizalama
        float z = Mathf.Round(transform.rotation.eulerAngles.z / 90f) * 90f;
        transform.rotation = Quaternion.Euler(0, 0, z);

        isMoving = false;

        if (shouldFlipDirection)
        {
            moveDirection *= -1;
            shouldFlipDirection = false;
        }
    }

    Vector3 GetPivotPoint()
    {
        Vector3 offset = Vector3.zero;

        if (moveDirection == Vector2.right)
            offset = new Vector3(0.5f, -0.5f, 0f);
        else if (moveDirection == Vector2.left)
            offset = new Vector3(-0.5f, -0.5f, 0f);

        return transform.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            shouldFlipDirection = true; // hemen çevirmiyoruz!
        }
    }


}
