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

    // Jika slot sudah dibuat secara manual di dalam Grid, gunakan yang sudah ada:
    int existingSlots = parent.childCount;
    if (existingSlots >= slotCount)
    {
        itemSlot = new ItemSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            itemSlot[i] = parent.GetChild(i).GetComponent<ItemSlot>();
        }
    }
    else
    {
        // Jika belum ada, buat secara otomatis dan pastikan berurutan
        itemSlot = new ItemSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            itemSlot[i] = Instantiate(slotPrefab, parent);
        }
    }
}

    public void AddItem(ItemDetails item)
{
    if (item == null)
        return;

    Debug.Log("Item added: " + item.ItemName);

    if (itemSlot == null || itemSlot.Length == 0)
        return;

    // 1. Cari slot KOSONG PERTAMA secara berurutan dari indeks 0 (Pojok Kiri Atas)
    for (int i = 0; i < itemSlot.Length; i++)
    {
        // Pastikan slot tidak null dan benar-benar kosong
        if (itemSlot[i] != null && itemSlot[i].IsEmpty)
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

        if (slot.selectedPanel != null)
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
            if (slot.selectedPanel != null)
                slot.selectedPanel.SetActive(false);
        }
    }
}