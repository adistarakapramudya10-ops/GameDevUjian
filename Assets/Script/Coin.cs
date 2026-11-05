using UnityEngine;

public class Coin : MonoBehaviour
{
    // Dipanggil otomatis saat objek lain masuk ke area Collider koin ini
    // Syarat: Koin harus punya Collider2D dengan "Is Trigger" tercentang
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Mengecek apakah yang menabrak adalah Player
        // Syarat: Objek Player harus punya Tag "Player" di Unity Inspector
        if (collision.CompareTag("Player"))
        {
            // Mencari GameManager di dalam Scene
            GameManager gameManager = FindObjectOfType<GameManager>();
            
            // Jika ketemu, panggil fungsi AmbilKoin()
            if (gameManager != null)
            {
                gameManager.AmbilKoin();
            }
            
            // Hancurkan objek koin dari Scene (koin menghilang)
            Destroy(gameObject);
        }
    }
}