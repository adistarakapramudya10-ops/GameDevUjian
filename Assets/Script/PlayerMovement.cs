using UnityEngine;
using UnityEngine.InputSystem;

namespace PASGame
{
    public class PlayerMovement : MonoBehaviour, IDamageable
    {
        public float kecepatan = 5f;
        public int skor = 0;
        public GameManager gameManager;

        private Rigidbody2D rb;
        private Animator animator; 
        private SpriteRenderer spriteRenderer; 
        private Vector2 moveInput;

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
            Debug.Log("Event spasi diterima!");
        }

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            moveInput = Vector2.zero;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed) moveInput.x = -1f;
                if (Keyboard.current.dKey.isPressed) moveInput.x = 1f;
                if (Keyboard.current.wKey.isPressed) moveInput.y = 1f;
                if (Keyboard.current.sKey.isPressed) moveInput.y = -1f;
            }
            moveInput.Normalize();

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                SerangMusuh();
            }

            bool isMoving = moveInput.magnitude > 0.1f;
            if (animator != null)
            {
                animator.SetBool("Moving", isMoving);
                if (isMoving)
                {
                    animator.SetFloat("X", moveInput.x);
                    animator.SetFloat("Y", moveInput.y);
                    if (spriteRenderer != null)
                    {
                        if (moveInput.x < 0) spriteRenderer.flipX = true;
                        else if (moveInput.x > 0) spriteRenderer.flipX = false;
                    }
                }
            }
        }

        void FixedUpdate()
        {
            rb.MovePosition(rb.position + moveInput * kecepatan * Time.fixedDeltaTime);
        }

        private void SerangMusuh()
        {
            if (animator != null) animator.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 2f);
            foreach (Collider2D enemyCollider in hitEnemies)
            {
                IDamageable damageable = enemyCollider.GetComponentInParent<IDamageable>();
                if (damageable != null && enemyCollider.gameObject != gameObject)
                {
                    damageable.KenaDamage(50);
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Coin"))
            {
                Destroy(other.gameObject);
                skor++;
                if (gameManager != null) gameManager.AmbilKoin();
            }
        }

        public void KenaDamage(int jumlah)
        {
            // Dikosongkan sengaja, karena urusan damage sekarang 100% dipegang oleh script PlayerHealth
        }
    }
}