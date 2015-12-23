using UnityEngine;
using System.Collections;

public class Projectile : Entity
{
    TrailRenderer tr;
    float trTime = 0;
    private float _trailTimer = 0.1f;

    void Awake()
    {
        tr = gameObject.GetComponent<TrailRenderer>();
        trTime = tr.time;
    }

    void Update()
    {
        Move();
        CheckBounds();

        if (!MapGenerator.Instance.tm.isAccesible(transform.position))
        {
            StartCoroutine(Deactivate());
        }

        if (tr.time == 0)
        {
            _trailTimer -= 1 * Time.deltaTime;
            if (_trailTimer <= 0)
            {
                tr.time = trTime;
                _trailTimer = 0.1f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (gameObject.tag == "PlayerBullet" && col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.SendMessage("TakeDemage", DMG, SendMessageOptions.DontRequireReceiver);
            StartCoroutine(Deactivate());
        }
        else if (gameObject.tag == "EnemyBullet" && col.gameObject.CompareTag("Player"))
        {
            Player.Instance.TakeDemage(DMG);
            StartCoroutine(Deactivate());
        }
    }

    void Move()
    {
        transform.Translate(transform.up * Speed * Time.deltaTime, Space.World);
    }

    void CheckBounds()
    {
        if (transform.position.x > MapGenerator.Instance.W * 32 || transform.position.x < 0 ||
            transform.position.y > MapGenerator.Instance.H * 32 || transform.position.y < 0)
            StartCoroutine(Deactivate());
    }

    IEnumerator Deactivate()
    {
        tr.time = 0;
        yield return new WaitForSeconds(0.01f);
        gameObject.SetActive(false);
    }
}
