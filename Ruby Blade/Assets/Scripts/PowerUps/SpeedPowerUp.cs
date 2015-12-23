using UnityEngine;
using System.Collections;

public class SpeedPowerUp : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.SendMessage("IncreaseSpeed", 10f, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }
}
