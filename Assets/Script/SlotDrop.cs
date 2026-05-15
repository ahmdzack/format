using UnityEngine;
using UnityEngine.EventSystems;

public class SlotDrop : MonoBehaviour, IDropHandler
{
    // Pilihan tipe slot (nanti bisa diatur dari Inspector)
    public enum TipeSlot { Angka, Operasi }
    public TipeSlot tipeSlotMenerima;

    public void OnDrop(PointerEventData eventData)
    {
        // Mengecek apakah ada objek yang sedang di-drag ke atas slot ini
        if (eventData.pointerDrag != null)
        {
            CardVisual kartu = eventData.pointerDrag.GetComponent<CardVisual>();
            
            if (kartu != null)
            {
                // Cek apakah slotnya masih kosong (belum ada kartu lain di dalamnya)
                if (transform.childCount == 0)
                {
                    // VALIDASI RULES:
                    // 1. Slot Angka hanya menerima kartu angka
                    if (tipeSlotMenerima == TipeSlot.Angka && kartu.isKartuAngka) {
                        TerimaKartu(kartu);
                    } 
                    // 2. Slot Operasi hanya menerima kartu non-angka
                    else if (tipeSlotMenerima == TipeSlot.Operasi && !kartu.isKartuAngka) {
                        TerimaKartu(kartu);
                    }
                    else {
                        Debug.Log("Tetot! Kartu ini nggak cocok masuk ke slot ini.");
                    }
                }
                else
                {
                    Debug.Log("Slot sudah penuh!");
                }
            }
        }
    }

    void TerimaKartu(CardVisual kartu)
    {
        // Jadikan slot ini sebagai rumah baru si kartu
        kartu.transform.SetParent(transform);
        
        // Posisikan pas di tengah slot
        kartu.transform.localPosition = Vector3.zero; 
    }
}