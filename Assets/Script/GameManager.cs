using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalKoin;
    private int koinTerkumpul = 0;
    private int jumlahZombieMati = 0;
    private bool isMenang = false;

    void OnEnable()
    {
        Enemy.OnZombieMati += SaatZombieMati;
    }

    void OnDisable()
    {
        Enemy.OnZombieMati -= SaatZombieMati;
    }

    void Start()
    {
        totalKoin = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    void SaatZombieMati(Enemy zombie)
    {
        jumlahZombieMati++;
        Debug.Log("GameManager dengar event. Zombie mati: " + jumlahZombieMati + " (" + zombie.name + ")");
    }

    public void AmbilKoin()
    {
        koinTerkumpul++;
        if (koinTerkumpul >= totalKoin && totalKoin > 0)
        {
            Menang();
        }
    }

    void Menang()
    {
        isMenang = true;
        Debug.Log("KAMU MENANG!");
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 22;
        GUI.skin.label.alignment = TextAnchor.UpperLeft; // Merapikan posisi default teks atas
        GUI.Label(new Rect(16, 16, 480, 36), "Koin: " + koinTerkumpul + " / " + totalKoin);
        GUI.Label(new Rect(16, 52, 480, 36), "Zombie mati: " + jumlahZombieMati);

        // Menampilkan teks "KAMU MENANG!" di tengah layar
        if (isMenang)
        {
            GUI.skin.label.fontSize = 40;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter; // Merapikan teks di tengah kotak

            // Lebar kotak diperbesar dari 300 ke 600 piksel biar muat
            GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 30, 600, 60), "KAMU MENANG!");
        }
    }
}