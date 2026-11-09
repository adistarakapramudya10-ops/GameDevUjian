using UnityEngine;

[CreateAssetMenu(fileName = "ItemDetails", menuName = "Scriptable Objects/ItemDetails")]
public class ItemDetails : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private int quantity = 1;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private Color itemColor = Color.white; // Warna unik per item
    [SerializeField, TextArea] private string description;

    public string ItemName => itemName;
    public int Quantity => quantity;
    public Sprite ItemIcon => itemIcon;
    public Color ItemColor => itemColor;
    public string Description => description;
}