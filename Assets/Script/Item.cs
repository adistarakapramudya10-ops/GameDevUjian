using UnityEngine;

public class Item : MonoBehaviour
{
    ItemDetails itemDetails;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OncollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has collided with the item");
            GrabItem();
        }
    }

    public void GrabItem()
    {
        Debug.Log("Item has been grabbed");
        // Add logic to add the item to the player's inventory
        Destroy(gameObject); // Remove the item from the scene after grabbing
    }
}
