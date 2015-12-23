using UnityEngine;
using System.Collections;

public class HPPowerUp : MonoBehaviour
{
    public float HPToAdd;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.SendMessage("AddHP", HPToAdd, SendMessageOptions.DontRequireReceiver);

            Destroy(gameObject);
        }
    }
}
