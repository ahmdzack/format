using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardVisual : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string idKartu;
    public bool isKartuAngka; 
    public bool bisaDiDrag = true; // TAMBAHAN BARU

    private Transform parentAsli;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetupKartu(string id, Sprite gambar)
    {
        idKartu = id;
        isKartuAngka = int.TryParse(id, out _);
        if (gambar != null) GetComponent<Image>().sprite = gambar;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!bisaDiDrag) return; // PENGAMAN: Kalau false, jangan bereaksi
        parentAsli = transform.parent; 
        transform.SetParent(transform.root); 
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!bisaDiDrag) return; // PENGAMAN
        transform.position = Input.mousePosition; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!bisaDiDrag) return; // PENGAMAN
        canvasGroup.blocksRaycasts = true; 
        if (transform.parent == transform.root) transform.SetParent(parentAsli);
    }
}