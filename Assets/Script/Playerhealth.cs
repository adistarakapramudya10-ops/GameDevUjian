using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Gunakan ini jika teks Game Over menggunakan TextMeshPro

namespace PASGame
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("HP")]
        public int maxHP = 5;                   

        [Header("UI")]
        public Image healthBarFill;             
        public GameObject gameOverPanel;        // Masukkan Panel/Teks Game Over dari Canvas di sini

        [Header("Respawn & kedip")]
        public Transform respawnPoint;          
        public float respawnDelay = 2f;
        public float spawnInvincibleTime = 2f;   
        public float hitInvincibleTime = 1f;     
        public float blinkInterval = 0.1f;

        [Header("Opsional (kosong = otomatis)")]
        public Behaviour[] disableWhileDead;    
        public SpriteRenderer[] blinkRenderers; 

        public int CurrentHP { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsInvincible { get; private set; }

        public event Action<int, int> OnHealthChanged;

        Vector3 startPosition;
        Quaternion startRotation; // Menyimpan rotasi awal player
        Rigidbody2D rb;

        void Awake()
        {
            if (maxHP < 1) maxHP = 1;
            CurrentHP = maxHP;
            startPosition = transform.position;
            startRotation = transform.rotation;
            rb = GetComponent<Rigidbody2D>();

            // Pastikan panel Game Over disembunyikan saat game dimulai
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            if (blinkRenderers == null || blinkRenderers.Length == 0)
            {
                blinkRenderers = GetComponentsInChildren<SpriteRenderer>();
            }

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
            UpdateHealthBarUI(); 
            StartCoroutine(InvincibleBlink(spawnInvincibleTime));   
        }

        public void KenaDamage(int amount)
        {
            if (amount <= 0 || IsDead || IsInvincible) return;

            CurrentHP = Mathf.Max(0, CurrentHP - amount);
            if (OnHealthChanged != null) OnHealthChanged(CurrentHP, maxHP);
            
            UpdateHealthBarUI(); 

            if (CurrentHP <= 0)
                StartCoroutine(DieAndRespawn());
            else
                StartCoroutine(InvincibleBlink(hitInvincibleTime));
        }

        public void TakeDamage(float amount)
        {
            KenaDamage(Mathf.RoundToInt(amount));
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0) return;
            CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
            if (OnHealthChanged != null) OnHealthChanged(CurrentHP, maxHP);
            
            UpdateHealthBarUI(); 
        }

        void UpdateHealthBarUI()
        {
            if (healthBarFill != null)
            {
                float targetFill = (float)CurrentHP / (float)maxHP;
                healthBarFill.fillAmount = targetFill;
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

            // --- ANIMASI SEDERHANA: Tengkurap (Memutar sprite player 90 atau 180 derajat) ---
            transform.rotation = Quaternion.Euler(0, 0, -90f); 

            // Tampilkan tulisan / Panel Game Over
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // Efek kedip-kedip saat mati sebelum respawn
            float elapsed = 0f;
            while (elapsed < respawnDelay)
            {
                SetVisible(false);
                yield return new WaitForSeconds(blinkInterval);
                SetVisible(true);
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval * 2f;
            }

            // --- RESET SETELAH RESPAWN ---
            transform.position = respawnPoint != null ? respawnPoint.position : startPosition;
            transform.rotation = startRotation; // Kembalikan posisi rotasi normal
            CurrentHP = maxHP;
            
            if (OnHealthChanged != null) OnHealthChanged(CurrentHP, maxHP);
            UpdateHealthBarUI(); 

            // Sembunyikan kembali panel Game Over
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

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