using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public GameObject playerBullet;
    public GameObject monsterBullet;

    public static BulletManager Instance;
    private ObjectPool playerBullets, monsterBullets;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerBullets = new ObjectPool(this);
        monsterBullets = new ObjectPool(this);

        playerBullets.Init(playerBullet);
        monsterBullets.Init(monsterBullet);
    }

    public void SpawnBullet(bool isMonster, Vector3 pos, Quaternion rotation)
    {
        GameObject b = isMonster ? monsterBullets.GetPooledObject() :
            playerBullets.GetPooledObject();
        b.SetActive(true);
        b.transform.position = pos;
        b.transform.rotation = rotation;
    }
}
