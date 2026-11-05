
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, GrabableItem
{
    public static InventoryManager Instance { get; private set; }

    public event Action OnInventoryChanged;

    InputAction inventoryAction;

    [SerializeField] private GameObject InventoryCanvas;
    [SerializeField] private int slotCount = 16;
    [SerializeField] private ItemDetailsPanel detailsPanel;

    private bool InventoryActive;

    public ItemSlot[] itemSlot;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateSlots();

        if (InventoryCanvas != null)
            InventoryCanvas.SetActive(false);

        inventoryAction = InputSystem.actions.FindAction("Inventory");

        InventoryActive = false;

        if (detailsPanel == null)
        {
            Debug.LogWarning("No ItemDetailsPanel assigned.", this);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (inventoryAction == null || InventoryCanvas == null)
            return;

        if (inventoryAction.triggered)
        {
            InventoryActive = !InventoryActive;

            Time.timeScale = InventoryActive ? 0f : 1f;

            InventoryCanvas.SetActive(InventoryActive);

            if (InventoryActive)
                ShowFirstItem();
        }
    }

    private void CreateSlots()
    {
        ItemSlot slotPrefab = Resources.Load<ItemSlot>("ItemSlot");

        if (slotPrefab == null)
        {
            Debug.LogError("ItemSlot prefab not found in Resources.");
            itemSlot = new ItemSlot[0];
            return;
        }

        Transform parent = InventoryCanvas.transform;

        GridLayoutGroup grid =
            InventoryCanvas.GetComponentInChildren<GridLayoutGroup>(true);

        if (grid != null)
            parent = grid.transform;

        itemSlot = new ItemSlot[slotCount];

        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i] = Instantiate(slotPrefab, parent);
        }
    }

    public void AddItem(ItemDetails item)
    {
        if (item == null)
            return;

        Debug.Log("Item added: " + item.ItemName);

        if (itemSlot == null || itemSlot.Length == 0)
            return;

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].CanStackWith(item.ItemName))
            {
                itemSlot[i].AddQuantity(item.Quantity);

                OnInventoryChanged?.Invoke();
                return;
            }
        }

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].IsEmpty)
            {
                itemSlot[i].AddItem(item);

                OnInventoryChanged?.Invoke();
                return;
            }
        }

        Debug.LogWarning("Inventory is full.");
    }

    public int GetAmount(ItemDetails item)
    {
        if (item == null || itemSlot == null)
            return 0;

        int total = 0;

        foreach (ItemSlot slot in itemSlot)
        {
            if (slot != null && !slot.IsEmpty &&
                slot.ItemDetails == item)
            {
                total += slot.itemQuantity;
            }
        }

        return total;
    }

    public void SelectSlot(ItemSlot slot)
    {
        DesselectAllSlot();

        slot.selectedPanel.SetActive(true);
        slot.isItemSelected = true;

        if (detailsPanel != null)
        {
            detailsPanel.Show(slot.ItemDetails, slot.itemQuantity);
        }
    }

    private void ShowFirstItem()
    {
        if (itemSlot != null)
        {
            foreach (ItemSlot slot in itemSlot)
            {
                if (slot != null && !slot.IsEmpty)
                {
                    SelectSlot(slot);
                    return;
                }
            }
        }

        if (detailsPanel != null)
            detailsPanel.Clear();
    }

    public void DesselectAllSlot()
    {
        if (itemSlot == null)
            return;

        foreach (ItemSlot slot in itemSlot)
        {
            if (slot == null)
                continue;

            slot.isItemSelected = false;
            slot.selectedPanel.SetActive(false);
        }
    }
}
