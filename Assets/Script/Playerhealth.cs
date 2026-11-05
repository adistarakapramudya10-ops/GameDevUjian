using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PASGame
{
    // Taruh di Player.
    // HP habis -> player hilang sebentar -> respawn di titik awal dengan efek kedip-kedip (kebal sementara).
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("HP")]
        public int maxHP = 5;                    // 5 HP = 6 gambar healthbar (5,4,3,2,1,0)

        [Header("UI")]
        public Image healthBarFill;              // 2. Tempat untuk memasukkan potongan UI Bar Penuh

        [Header("Respawn & kedip")]
        public Transform respawnPoint;           // kosong = kembali ke posisi awal player
        public float respawnDelay = 1f;
        public float spawnInvincibleTime = 2f;   // kedip + kebal setelah respawn & di awal game
        public float hitInvincibleTime = 1f;     // kedip + kebal setelah kena hit
        public float blinkInterval = 0.1f;

        [Header("Opsional (kosong = otomatis)")]
        public Behaviour[] disableWhileDead;     // script yang dimatikan saat mati
        public SpriteRenderer[] blinkRenderers;  // sprite yang berkedip

        public int CurrentHP { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsInvincible { get; private set; }

        // (hpSekarang, hpMaks) -> didengarkan HealthBarUI
        public event Action<int, int> OnHealthChanged;

        Vector3 startPosition;
        Rigidbody2D rb;

        void Awake()
        {
            if (maxHP < 1) maxHP = 1;
            CurrentHP = maxHP;
            startPosition = transform.position;
            rb = GetComponent<Rigidbody2D>();

            if (blinkRenderers == null || blinkRenderers.Length == 0)
            {
                blinkRenderers = GetComponentsInChildren<SpriteRenderer>();
            }

            // Otomatis: matikan PlayerMovement* dan PlayerShooter saat mati
            if (disableWhileDead == null || disableWhileDead.Length == 0)
            {
                List<Behaviour> found = new List<Behaviour>();
                MonoBehaviour[] all = GetComponents<MonoBehaviour>();
                for (int i = 0; i < all.Length; i++)
                {
                    MonoBehaviour mb = all[i];
                    if (mb == null || mb == this) continue;
                    string typeName = mb.GetType().Name;
                    if (typeName.StartsWith("PlayerMovem") || typeName == "PlayerShooter")
                    {
                        found.Add(mb);
                    }
                }
                disableWhileDead = found.ToArray();
            }
        }

        void Start()
        {
            if (OnHealthChanged != null) OnHealthChanged(CurrentHP, maxHP);
            UpdateHealthBarUI(); // Perbarui bar saat game dimulai
            StartCoroutine(InvincibleBlink(spawnInvincibleTime));   // efek spawn di awal game
        }

        public void KenaDamage(int amount)
        {
            if (amount <= 0 || IsDead || IsInvincible) return;

            CurrentHP = Mathf.Max(0, CurrentHP - amount);
            if (OnHealthChanged != null) OnHealthChanged(CurrentHP, maxHP);
            
            UpdateHealthBarUI(); // Perbarui bar saat terkena damage

            if (CurrentHP <= 0)
                StartCoroutine(DieAndRespawn());
            else
                StartCoroutine(InvincibleBlink(hitInvincibleTime));
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0) return;
            CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
            if (OnHealthChanged != null) OnHealthChanged(CurrentHP, maxHP);
            
            UpdateHealthBarUI(); // Perbarui bar saat darah bertambah
        }

        // Fungsi memotong panjang bar UI sesuai sisa HP (Dengan Pelacak)
        void UpdateHealthBarUI()
        {
            if (healthBarFill != null)
            {
                // Memastikan pembagian menggunakan angka desimal (float)
                float targetFill = (float)CurrentHP / (float)maxHP;
                healthBarFill.fillAmount = targetFill;
                
                Debug.Log("UI UPDATE: HP sekarang " + CurrentHP + "/" + maxHP + " | Fill Amount diatur ke: " + targetFill);
            }
            else
            {
                Debug.LogWarning("UI UPDATE GAGAL: Kolom Health Bar Fill di Inspector masih kosong!");
            }
        }

        IEnumerator DieAndRespawn()
        {
            IsDead = true;
            SetControlEnabled(false);
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            SetVisible(false);

            yield return new WaitForSeconds(respawnDelay);

            transform.position = respawnPoint != null ? respawnPoint.position : startPosition;
            CurrentHP = maxHP;
            if (OnHealthChanged != null) OnHealthChanged(CurrentHP, maxHP);
            
            UpdateHealthBarUI(); // Kembalikan bar menjadi penuh saat respawn

            IsDead = false;
            SetControlEnabled(true);
            yield return StartCoroutine(InvincibleBlink(spawnInvincibleTime));
        }

        IEnumerator InvincibleBlink(float duration)
        {
            IsInvincible = true;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                SetVisible(false);
                yield return new WaitForSeconds(blinkInterval);
                SetVisible(true);
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval * 2f;
            }
            SetVisible(true);
            IsInvincible = false;
        }

        void SetVisible(bool visible)
        {
            if (blinkRenderers == null) return;
            for (int i = 0; i < blinkRenderers.Length; i++)
            {
                if (blinkRenderers[i] != null) blinkRenderers[i].enabled = visible;
            }
        }

        void SetControlEnabled(bool value)
        {
            if (disableWhileDead == null) return;
            for (int i = 0; i < disableWhileDead.Length; i++)
            {
                if (disableWhileDead[i] != null) disableWhileDead[i].enabled = value;
            }
        }
    }
}