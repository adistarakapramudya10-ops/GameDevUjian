using UnityEngine;
using System.Collections; // Dibutuhkan untuk IEnumerator (Jeda Waktu)

namespace PASGame
{
    public class EnemyChildren : MonoBehaviour
    {
        [Header("Mode Script")]
        [Tooltip("Centang jika objek ini adalah Spawner. Hilangkan centang jika objek ini adalah Roket.")]
        public bool isSpawner = false;

        [Header("Pengaturan Spawner (Jika dicentang)")]
        [SerializeField] private GameObject rocketPrefab; // Masukkan prefab roket ke sini
        [SerializeField] private float jedaSpawn = 0.5f;

        [Header("Pengaturan Roket (Jika TIDAK dicentang)")]
        [SerializeField] private float kecepatanKejar = 4f;
        [SerializeField] private int damageToPlayer = 50; // Ubah damage sesuai kebutuhan

        private Transform playerTarget;

        private void Start()
        {
            if (isSpawner)
            {
                // MODE SPAWNER: Memulai proses memunculkan 3-5 roket
                StartCoroutine(MunculkanRoket());
            }
            else
            {
                // MODE ROKET: Mencari target Player saat roket muncul
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerTarget = playerObj.transform;
                }
            }
        }

        private IEnumerator MunculkanRoket()
        {
            // Menentukan jumlah roket acak (3 sampai 5)
            int jumlahRoket = Random.Range(2, 4); 

            for (int i = 0; i < jumlahRoket; i++)
            {
                if (rocketPrefab != null)
                {
                    // Menentukan posisi acak di sekitar spawner
                    Vector2 posisiAcak = (Vector2)transform.position + Random.insideUnitCircle * 1.5f;
                    Instantiate(rocketPrefab, posisiAcak, Quaternion.identity);
                }
                
                // Jeda sebelum memunculkan roket berikutnya
                yield return new WaitForSeconds(jedaSpawn);
            }

            // (Opsional) Hancurkan spawner setelah semua roket keluar
            Destroy(gameObject);
        }

        private void Update()
        {
            // MODE ROKET: Hanya roket yang bergerak mengejar target
            if (!isSpawner && playerTarget != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerTarget.position, kecepatanKejar * Time.deltaTime);

                // Rotasi menghadap player
                Vector2 arah = playerTarget.position - transform.position;
                float sudut = Mathf.Atan2(arah.y, arah.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, sudut);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // MODE ROKET: Hanya roket yang bisa meledak saat menabrak Player
            if (!isSpawner && collision.gameObject.CompareTag("Player"))
            {
                // CARI PLAYER HEALTH (Sama seperti musuh biasa)
                PlayerHealth playerHp = collision.gameObject.GetComponent<PlayerHealth>();
                
                if (playerHp != null)
                {
                    playerHp.KenaDamage(damageToPlayer);
                    Debug.Log("Roket menabrak Player dan memberikan damage!");
                }

                Destroy(gameObject); // Roket hancur (meledak)
            }
        }
    }
}