using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    public float kecepatan = 5f;
    public int skor = 0;

    public GameManager gameManager;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // --- INTEGRASI EVENT RECEIVER (POIN 5 TUGAS) ---
    private void OnEnable()
    {
        PengirimEvent.OnTekanSpasi += TerimaNotifikasiSpasi;
    }

    private void OnDisable()
    {
        PengirimEvent.OnTekanSpasi -= TerimaNotifikasiSpasi;
    }

    private void TerimaNotifikasiSpasi()
    {
        Debug.Log("Penerima Event (Player): Event spasi diterima!");
    }
    // ----------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            // Pergerakan WASD
            if (Keyboard.current.aKey.isPressed) moveInput.x = -1;
            if (Keyboard.current.dKey.isPressed) moveInput.x = 1;
            if (Keyboard.current.wKey.isPressed) moveInput.y = 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y = -1;

            // Tekan Spasi untuk menyerang Enemy di sekitar
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                SerangMusuh();
            }
        }

        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * kecepatan * Time.fixedDeltaTime);
    }

    // Mekanisme Player memukul Enemy (Radius jangkauan 5f agar pasti kena)
    private void SerangMusuh()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 5f);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            IDamageable damageable = enemyCollider.GetComponentInParent<IDamageable>();

            if (damageable != null && enemyCollider.gameObject != gameObject)
            {
                damageable.KenaDamage(50); // Kasih 50 damage ke Enemy
                Debug.Log("Player memukul enemy!");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            skor++;
            Debug.Log("Skor : " + skor);

            if (gameManager != null)
            {
                gameManager.AmbilKoin();
            }
        }
    }

    // Player bisa kena damage (Syarat Interface IDamageable) tapi TIDAK BISA MATI karena tidak punya HP
    public void KenaDamage(int jumlah)
    {
        Debug.Log("Player kena damage sebesar: " + jumlah);
    }
}