using UnityEngine;
using System;
using PASGame; // Ditambahkan agar Enemy bisa membaca sistem Player dari namespace PASGame

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp = 100;
    public float ms = 2f;

    // Damage yang diterima Player setiap kali ditabrak. Diset 1 karena Max HP Player adalah 5.
    [SerializeField] private int damageSaatTabrakan = 1;

    // Fitur Patroli
    [SerializeField] private float radiusPatrol = 3f;
    private Vector2 titikAwal;
    private Vector2 tujuanPatrol;

    protected Transform player;

    [Header("State Machine")]
    [SerializeField] private float JarakDeteksi = 6f;
    [SerializeField] private float JarakSerang = 1.5f;
    [SerializeField] private float JedaSerang = 1f;

    // State Now
    private StateZombie state = StateZombie.IDLE;
    private float waktuSerangTerakhir;

    public static event Action<Enemy> OnZombieMati;

    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Inisialisasi posisi awal dan titik patroli pertama
        titikAwal = transform.position;
        PilihTujuanPatrolBaru();
    }

    void Update()
    {
        PeriksaTransisi();
        
        switch(state)
        {
            case StateZombie.IDLE: PerilakuIdle(); break;
            case StateZombie.PATROL: PerilakuPatrol(); break;
            case StateZombie.CHASE: PerilakuChase(); break;
            case StateZombie.ATTACK: PerilakuAttack(); break;
        }
    }

    public float JarakKePlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }

    void PeriksaTransisi()
    {
        float jarak = JarakKePlayer();

        if (jarak <= JarakSerang)
        {
            state = StateZombie.ATTACK;
        }
        else if (jarak <= JarakDeteksi)
        {
            state = StateZombie.CHASE;
        }
        else
        {
            state = StateZombie.PATROL;
        }   
    }

    public void Kejar()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            ms * Time.deltaTime
        );
    }

    public virtual void Serang()
    {
        // Debug.Log("Enemy menyerang!");
    }

    void PerilakuIdle()
    {
        // Log dimatikan agar Console tidak lag
    }

    void PerilakuPatrol()
    {
        // Pergerakan jalan patroli
        transform.position = Vector2.MoveTowards(
            transform.position, tujuanPatrol, ms * 0.5f * Time.deltaTime);

        if (Vector2.Distance(transform.position, tujuanPatrol) < 0.1f)
            PilihTujuanPatrolBaru();
    }

    void PilihTujuanPatrolBaru()
    {
        Vector2 acak = UnityEngine.Random.insideUnitCircle * radiusPatrol;
        tujuanPatrol = titikAwal + acak;
    }

    void PerilakuChase()
    {
        Kejar();
    }

    void PerilakuAttack()
    {
        if (Time.time >= waktuSerangTerakhir + JedaSerang)
        {
            Serang();
            waktuSerangTerakhir = Time.time;
        }
    }

    public void KenaDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Enemy kena damage: " + damage + ", HP sekarang: " + hp);
        
        if (hp <= 0)
        {
            Mati();
        }
    }

    protected virtual void Mati()
    {
        Debug.Log("Enemy mati!");
        
        // Memanggil event OnZombieMati agar GameManager tahu zombie ini mati
        OnZombieMati?.Invoke(this); 
        
        Destroy(gameObject);
    }

    // Dipanggil otomatis oleh Unity saat dua benda padat (Collider 2D) saling bertabrakan
    // Dipanggil otomatis oleh Unity saat dua benda padat (Collider 2D) saling bertabrakan
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Mengecek apakah objek yang ditabrak memiliki Tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Langsung akses komponen PlayerHealth yang ada di Player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                // Memberikan damage ke Player
                playerHealth.KenaDamage(damageSaatTabrakan);
                Debug.Log("Musuh berhasil menabrak dan melukai Player sebesar: " + damageSaatTabrakan);
            }
            else 
            {
                Debug.LogWarning("Player ditabrak tapi script PlayerHealth tidak ditemukan di Player!");
            }
        }
    }
}