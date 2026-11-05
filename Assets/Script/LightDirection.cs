using UnityEngine;

public class LightDirection : MonoBehaviour
{
    void Update()
    {
        // 1. Mengambil input arah gerak player (tombol WASD atau panah)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        // 2. Mengecek apakah player sedang menekan tombol gerak
        if (moveDirection != Vector2.zero)
        {
            // 3. Menghitung sudut rotasi berdasarkan arah X dan Y
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            // 4. Memutar objek lampu pada sumbu Z
            // Dikurangi 90 karena arah default Spot Light di Unity menghadap ke atas (Y positif)
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
}