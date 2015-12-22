using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip Fall;

    bool isFalling = false;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        audio.Play();
    }


    // Update is called once per frame
    void Update()
    {
        if(Ball.Instance.gameObject.transform.position.y < 0 && !isFalling)
        {
            StartCoroutine(FallE());
        }
    }

    IEnumerator FallE()
    {
        isFalling = true;
        audio.PlayOneShot(Fall);
        yield return new WaitForSeconds(2);
        isFalling = false;
    }
}
