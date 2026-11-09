using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // Item Data
    public string itemName;
    public int itemQuantity;
    public Sprite itemIcon;
    public Color itemColor = Color.white;
    public bool isFull;

    // Item Slot UI Components
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] private int maxStackSize = 99;

    // Selected Item
    [SerializeField] public GameObject selectedPanel;
    public bool isItemSelected;

    // The full item data stored in this slot
    public ItemDetails ItemDetails => itemDetails;
    private ItemDetails itemDetails;

    public InventoryManager inventoryManager;

    public void Start()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        
        if (selectedPanel != null)
            selectedPanel.SetActive(false);
            
        UpdateSlotUI();
    }

    public void AddItem(ItemDetails item)
    {
        itemDetails = item;
        itemName = item.ItemName;
        itemIcon = item.ItemIcon;
        itemColor = item.ItemColor; // Mengambil warna dari item details
        AddQuantity(item.Quantity);
    }

    public bool IsEmpty => itemQuantity <= 0 || itemDetails == null;

    public bool CanStackWith(string otherItemName)
    {
        return !isFull && itemName == otherItemName;
    }

    public void AddQuantity(int amount)
    {
        itemQuantity += amount;
        isFull = itemQuantity >= maxStackSize;
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        if (IsEmpty)
        {
            if (itemImage != null)
            {
                itemImage.sprite = null;
                itemImage.enabled = false;
            }
            if (quantityText != null)
            {
                quantityText.text = "";
                quantityText.enabled = false;
            }
        }
        else
        {
            if (itemImage != null)
            {
                itemImage.sprite = itemIcon;
                itemImage.enabled = true;
                
                // Menerapkan warna unik milik item tersebut dan memastikan tidak transparan
                itemImage.color = itemColor;
                Color col = itemImage.color;
                col.a = 1f;
                itemImage.color = col;
            }

            if (quantityText != null)
            {
                quantityText.text = itemQuantity > 1 ? itemQuantity.ToString() : "";
                quantityText.enabled = true;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClick();
        }
    }

    private void LeftClick()
    {
        if (IsEmpty || inventoryManager == null) return;
        inventoryManager.SelectSlot(this);
    }

    private void RightClick()
    {
        Debug.Log("Right Clicked on " + itemName);
    }
}