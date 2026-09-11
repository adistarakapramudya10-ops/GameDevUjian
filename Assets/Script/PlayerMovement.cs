using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    public float kecepatan = 5f;
    public int skor = 0;

    public GameManager gameManager;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                moveInput.x = -1;

            if (Keyboard.current.dKey.isPressed)
                moveInput.x = 1;

            if (Keyboard.current.wKey.isPressed)
                moveInput.y = 1;

            if (Keyboard.current.sKey.isPressed)
                moveInput.y = -1;
        }

        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * kecepatan * Time.fixedDeltaTime);
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

    // Implementasi interface IDamageable
    public void KenaDamage(int jumlah)
    {
        Debug.Log("Player kena damage sebesar: " + jumlah);
    }
}