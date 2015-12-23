using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip BackgroundMusic,
                    soldierIntro,
                    PowerUp, Shield,
                    Gun,
                    TakeDmg;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        audio.PlayOneShot(soldierIntro);
        audio.PlayDelayed(4);
    }

    public void PlayPowerUp()
    {
        audio.PlayOneShot(PowerUp);
    }
    public void PlayShield()
    {
        audio.PlayOneShot(Shield);
    }
    public void PlayGun()
    {
        audio.PlayOneShot(Gun, 0.25f);
    }
}
