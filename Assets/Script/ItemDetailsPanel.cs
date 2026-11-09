using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDetailsPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelParent;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemQuantityText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private Image itemIconImage;

    void Start()
    {
        if (panelParent != null)
            panelParent.SetActive(false); // Sembunyikan panel di awal game
    }

    public void Show(ItemDetails item, int quantity)
    {
    if (item == null) return;

    if (panelParent != null) panelParent.SetActive(true);

    if (itemNameText != null) itemNameText.text = item.ItemName;
    if (itemQuantityText != null) itemQuantityText.text = "Qty: " + quantity;
    if (itemDescriptionText != null) itemDescriptionText.text = item.Description;

    if (itemIconImage != null)
        {
        itemIconImage.sprite = item.ItemIcon;
        itemIconImage.enabled = (item.ItemIcon != null);
        
        // Mengambil warna dari ItemDetails
        itemIconImage.color = item.ItemColor;
        
        // Memastikan tidak transparan
        Color col = itemIconImage.color;
        col.a = 1f;
        itemIconImage.color = col;
        }
    }

    public void Clear()
    {
        if (panelParent != null) panelParent.SetActive(false);
    }
}