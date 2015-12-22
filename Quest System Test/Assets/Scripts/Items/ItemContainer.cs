using UnityEngine;

[RequireComponent(typeof(Item))]
[RequireComponent(typeof(SphereCollider))]
public class ItemContainer : MonoBehaviour
{
    private Inventory inventory;
    void Start()
    {
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            UIHandler.Instance.ShowItemAddedWindow(GetComponent<Item>());
            inventory.AddItem(GetComponent<Item>());
        }
    }
}
