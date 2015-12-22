using UnityEngine;
using System.Collections;

public class ShieldPowerUp : MonoBehaviour
{
    private float[] parameters = { 10f, 300f };

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            col.SendMessage("ActivateShield", parameters, SendMessageOptions.DontRequireReceiver);
            SoundManager.Instance.PlayShield();
            Destroy(gameObject);
        }
    }
}
