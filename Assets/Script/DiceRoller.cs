using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Wajib ditambahkan untuk memanggil komponen UI

public class DiceRoller : MonoBehaviour
{
    [Header("Masukkan 10 Sprite Dadu (0-9) di Sini")]
    public Sprite[] daduSprites; 
    
    public int hasilAngka = 0; 
    public bool sudahBerhenti = false;

    private Image daduImage; // Berubah dari SpriteRenderer menjadi Image
    private bool sedangDikocok = false;

    void Start()
    {
        daduImage = GetComponent<Image>(); // Mengambil komponen UI Image
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !sedangDikocok)
        {
            StartCoroutine(AnimasiPutarDanBerhenti());
        }
    }

    private IEnumerator AnimasiPutarDanBerhenti()
    {
        if (daduSprites.Length == 0)
        {
            Debug.LogError("Error: Gambar dadu belum dimasukkan ke Inspector!");
            yield break; 
        }

        sedangDikocok = true;
        
        // Fase 1: Animasi berputar
        int jumlahPutaran = 20; 
        for (int i = 0; i < jumlahPutaran; i++)
        {
            int angkaVisual = Random.Range(0, daduSprites.Length);
            daduImage.sprite = daduSprites[angkaVisual]; // Pakai daduImage
            
            float jeda = 0.05f + (i * 0.005f);
            yield return new WaitForSeconds(jeda); 
        }

        // Fase 2: Berhenti dan catat angka
        int angkaFinal = Random.Range(0, daduSprites.Length);
        daduImage.sprite = daduSprites[angkaFinal]; // Pakai daduImage
        
        hasilAngka = angkaFinal; 
        sudahBerhenti = true;    
        
        Debug.Log("Dadu UI berhenti di: " + angkaFinal);
        sedangDikocok = false; 
    }
}