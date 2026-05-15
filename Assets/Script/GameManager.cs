using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using DG.Tweening;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referensi Objek UI - Posisi Deck")]
    public Transform posisiDeckMain;    
    public Transform posisiDeckSupport; 
    public Transform posisiDeckDamage; 

    [Header("Referensi Objek UI - Panel Utama")]
    public Transform panelOperationBoard; 
    public Transform panelHandMain;       
    public Transform panelHandSupport;    

    [Header("Referensi UI Pilihan Support")]
    public GameObject panelPilihanSupport; 
    public Transform containerPilihan;     

    [Header("Referensi UI Pilihan Joker")]
    public GameObject panelPilihanJoker; 
    public Transform containerJoker;     
    
    [Header("Referensi Dadu & Prefab")]
    public DiceRoller daduPuluhan;
    public DiceRoller daduSatuan;
    public GameObject cardPrefab; 

    [Header("Data Gambar Angka (1-10)")]
    public Sprite[] spriteAngka;
    public Sprite[] spriteAngkaSupport; 

    [Header("Data Gambar Face Card (Main Deck)")]
    public Sprite sprite_Reverse; 
    public Sprite sprite_Block;   
    public Sprite sprite_Op_Plus;
    public Sprite sprite_Op_Minus;
    public Sprite sprite_Op_Kali;
    public Sprite sprite_Op_Bagi;
    public Sprite sprite_Joker_End;

    [Header("Data Gambar Face Card (Support Deck)")] 
    public Sprite sprite_J_Support;
    public Sprite sprite_Q_Support;
    public Sprite sprite_K_Support;
    public Sprite sprite_Joker_Support;

    private List<string> mainDeck = new List<string>();
    private List<string> supportDeck = new List<string>();
    
    private bool sudahBagiKartu = false;
    private bool sudahPakaiSupport = false;

    void Awake() { Instance = this; }

    void Start()
    {
        // Pastikan semua panel popup mati saat game baru mulai
        if (panelPilihanSupport != null) panelPilihanSupport.SetActive(false); 
        if (panelPilihanJoker != null) panelPilihanJoker.SetActive(false); 
        
        InisialisasiDeck();
    }

    void InisialisasiDeck()
    {
        for (int i = 1; i <= 10; i++) supportDeck.Add(i.ToString());
        supportDeck.Add("J_Support"); supportDeck.Add("Q_Support"); supportDeck.Add("K_Support"); supportDeck.Add("Joker_Support");

        for (int i = 1; i <= 10; i++) {
            for (int j = 0; j < 8; j++) mainDeck.Add(i.ToString());
        }
        for (int i = 0; i < 4; i++) {
            mainDeck.Add("Spesial_Reverse"); mainDeck.Add("Spesial_Block"); 
        }
        for (int i = 0; i < 2; i++) {
            mainDeck.Add("Op_Plus"); mainDeck.Add("Op_Minus"); mainDeck.Add("Op_Kali"); mainDeck.Add("Op_Bagi");
        }
        mainDeck.Add("Spesial_EndCard");

        ShuffleDeck(mainDeck);
    }

    void ShuffleDeck(List<string> deck)
    {
        for (int i = 0; i < deck.Count; i++) {
            string temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void Update()
    {
        if (daduPuluhan.sudahBerhenti && daduSatuan.sudahBerhenti && !sudahBagiKartu)
        {
            sudahBagiKartu = true;
            StartCoroutine(FlowMulaiPermainan());
        }
    }

    IEnumerator FlowMulaiPermainan()
    {
        sudahPakaiSupport = false; 
        posisiDeckMain.DOShakePosition(0.5f, 10f);

        List<string> ditarik = new List<string>();
        for (int i = 0; i < 3; i++) {
            string kartuAngka = mainDeck.First(k => int.TryParse(k, out _));
            ditarik.Add(kartuAngka);
            mainDeck.Remove(kartuAngka);
        }

        string kartuOp = mainDeck.First(k => !int.TryParse(k, out _));
        ditarik.Add(kartuOp);
        mainDeck.Remove(kartuOp);

        ShuffleDeck(ditarik);

        foreach (string idKartu in ditarik)
        {
            DrawKartuSpesifik(idKartu, panelHandMain, posisiDeckMain, false);
            yield return new WaitForSeconds(0.15f);
        }
    }

    // --- LOGIKA PEMILIHAN SUPPORT CARD ---
    public void KlikDeckSupport()
    {
        if (sudahPakaiSupport) {
            Debug.Log("Sudah ambil Support Card ronde ini!"); return;
        }
        if (supportDeck.Count > 0) {
            posisiDeckSupport.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f);
            TampilkanMenuSupport();
        }
    }

    void TampilkanMenuSupport()
    {
        panelPilihanSupport.SetActive(true);

        foreach (Transform child in containerPilihan) {
            Destroy(child.gameObject);
        }

        foreach (string idKartu in supportDeck)
        {
            GameObject opsiBaru = Instantiate(cardPrefab, containerPilihan);
            CardVisual cv = opsiBaru.GetComponent<CardVisual>();
            
            Sprite gambarPas = null;
            if (int.TryParse(idKartu, out int angka)) {
                if (spriteAngkaSupport != null && spriteAngkaSupport.Length >= angka) gambarPas = spriteAngkaSupport[angka - 1];
            } else {
                if (idKartu == "J_Support") gambarPas = sprite_J_Support;
                else if (idKartu == "Q_Support") gambarPas = sprite_Q_Support;
                else if (idKartu == "K_Support") gambarPas = sprite_K_Support;
                else if (idKartu == "Joker_Support") gambarPas = sprite_Joker_Support;
            }

            cv.SetupKartu(idKartu, gambarPas);
            cv.bisaDiDrag = false; 

            Button btn = opsiBaru.GetComponent<Button>();
            if (btn == null) btn = opsiBaru.AddComponent<Button>();
            
            btn.onClick.RemoveAllListeners(); 
            string idPilihan = idKartu; 
            btn.onClick.AddListener(() => MemilihKartu(idPilihan));
        }
    }

    public void MemilihKartu(string idDitarik)
    {
        if (idDitarik == "Joker_Support") 
        {
            panelPilihanSupport.SetActive(false); 
            TampilkanMenuJoker();                 
            return; 
        }

        sudahPakaiSupport = true;
        panelPilihanSupport.SetActive(false); 
        supportDeck.Remove(idDitarik); 

        DrawKartuSpesifik(idDitarik, panelHandSupport, posisiDeckSupport, true);
    }

    // --- LOGIKA PERUBAHAN WUJUD JOKER ---
    void TampilkanMenuJoker()
    {
        panelPilihanJoker.SetActive(true);

        foreach (Transform child in containerJoker) {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= 10; i++)
        {
            string idAngka = i.ToString();
            GameObject opsiBaru = Instantiate(cardPrefab, containerJoker);
            CardVisual cv = opsiBaru.GetComponent<CardVisual>();
            
            Sprite gambarPas = null;
            if (spriteAngkaSupport != null && spriteAngkaSupport.Length >= i) {
                gambarPas = spriteAngkaSupport[i - 1];
            }

            cv.SetupKartu(idAngka, gambarPas);
            cv.bisaDiDrag = false;

            Button btn = opsiBaru.GetComponent<Button>();
            if (btn == null) btn = opsiBaru.AddComponent<Button>();
            
            btn.onClick.RemoveAllListeners(); 
            btn.onClick.AddListener(() => MemilihAngkaJoker(idAngka));
        }
    }

    public void MemilihAngkaJoker(string idAngkaBaru)
    {
        sudahPakaiSupport = true;
        panelPilihanJoker.SetActive(false); 
        
        supportDeck.Remove("Joker_Support"); 

        DrawKartuSpesifik(idAngkaBaru, panelHandSupport, posisiDeckSupport, true);
    }

    // ------------------------------------

    void DrawKartuSpesifik(string idDitarik, Transform targetPanel, Transform posisiAwal, bool isSupportCard)
    {
        GameObject kartuBaru = Instantiate(cardPrefab, targetPanel);
        Sprite gambarPas = null;

        if (int.TryParse(idDitarik, out int angka)) {
            if (isSupportCard) {
                if (spriteAngkaSupport != null && spriteAngkaSupport.Length >= angka) gambarPas = spriteAngkaSupport[angka - 1];
            } else {
                if (spriteAngka != null && spriteAngka.Length >= angka) gambarPas = spriteAngka[angka - 1];
            }
        } 
        else {
            if (idDitarik == "Spesial_Reverse") gambarPas = sprite_Reverse;
            else if (idDitarik == "Spesial_Block") gambarPas = sprite_Block;
            else if (idDitarik == "Op_Plus") gambarPas = sprite_Op_Plus;
            else if (idDitarik == "Op_Minus") gambarPas = sprite_Op_Minus;
            else if (idDitarik == "Op_Kali") gambarPas = sprite_Op_Kali;
            else if (idDitarik == "Op_Bagi") gambarPas = sprite_Op_Bagi;
            else if (idDitarik == "Spesial_EndCard") gambarPas = sprite_Joker_End;

            else if (idDitarik == "J_Support") gambarPas = sprite_J_Support;
            else if (idDitarik == "Q_Support") gambarPas = sprite_Q_Support;
            else if (idDitarik == "K_Support") gambarPas = sprite_K_Support;
            else if (idDitarik == "Joker_Support") gambarPas = sprite_Joker_Support;
        }

        kartuBaru.GetComponent<CardVisual>().SetupKartu(idDitarik, gambarPas);
        kartuBaru.transform.position = posisiAwal.position;
        kartuBaru.transform.localScale = Vector3.zero;
        kartuBaru.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }
}