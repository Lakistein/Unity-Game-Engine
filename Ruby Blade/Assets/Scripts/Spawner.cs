using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public static Spawner Instace;
    public const float SPAWN_SUBWAVE_TIME = 5f, SPAWN_WAVE_TIME = 30f, SPAWN_POWERUP_TIME_MIN = 15f, SPAWN_POWERUP_TIME_MAX = 30f, MAX_ENEMIES = 100;
    public GameObject type_1, type_2, type_3;
    public GameObject weapon, magazine;
    public byte currWave = 1;
    public int currNumOfEnemies = 0;
    public GameObject[] powerUps;
    private GUIText waveUI;
    private float subWaveTime = 0, waveTime = 0, weaponTime = 0, weaponTimeCurr = 0;
    private byte subWaveCount = 5;
    private GameObject goToSpawn;
    [HideInInspector]
    public GUIText NoOfEnem;

    void Awake()
    {
        Instace = this;
    }

    void Start()
    {
        waveUI = GameObject.Find("WaveCount").GetComponent<GUIText>();
        NoOfEnem = GameObject.Find("NoOfEnem").GetComponent<GUIText>();
        waveUI.text = "Current wave: " + currWave.ToString();
    }

    void Update()
    {
        SpawnWave();
        SpawnPoerUp();
    }

    void SpawnMonsters(int number)
    {
        int t2_count = number / 2, t3_count = number / 3;
        for (int i = 0; i < number; i++)
        {
            goToSpawn = new GameObject();

            if (currWave >= 3 && t3_count > 0)
            {
                goToSpawn = type_3;
                t3_count--;
            }
            else if (currWave >= 2 && t2_count > 0)
            {
                goToSpawn = type_2;
                t2_count--;
            }
            else
            {
                goToSpawn = type_1;
            }
            goToSpawn.transform.position = GetValidPosition();
            Instantiate(goToSpawn);
        }
        currNumOfEnemies += number;
        UpdateGUI();
    }

    void SpawnWave()
    {
        if (subWaveCount > 0)
        {
            subWaveTime += Time.deltaTime;
            if (subWaveTime > SPAWN_SUBWAVE_TIME)
            {
                SpawnMonsters(currWave * 3);
                subWaveTime -= SPAWN_SUBWAVE_TIME;
                subWaveCount--;
            }
        }

        waveTime += Time.deltaTime;
        if (waveTime > SPAWN_WAVE_TIME)
        {
            currWave++;
            waveTime = 0;
            subWaveCount = 4;
            UpdateGUI();
        }
    }

    void SpawnPoerUp()
    {
        weaponTimeCurr += Time.deltaTime;
        if (weaponTimeCurr > weaponTime)
        {
            SpawnPowerUp(GetValidPosition());
            weaponTimeCurr = 0;
            weaponTime = Random.Range(SPAWN_POWERUP_TIME_MIN, SPAWN_POWERUP_TIME_MAX);
        }
    }

    public void SpawnPowerUp(Vector3 pos)
    {
        int rnd = Random.Range(0, powerUps.Length);
        Instantiate(powerUps[rnd], pos, Quaternion.identity);
    }

    Vector3 GetValidPosition()
    {
        Vector3 pos = Vector3.zero;
        Vector2 camCurrPosLowLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector2 camCurrPosTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 1));

        bool spawnUp = false,
             spawnRight = false;

        bool spawnTopOrDown = Random.Range(0, 2) == 0;

        if (spawnTopOrDown)
            spawnUp = Random.Range(0, 2) == 0 ? ((camCurrPosTopRight.y < (MapGenerator.Instance.H - 1) * 32 - 32) ? true : false) : ((camCurrPosLowLeft.y > 32) ? false : true);
        else
            spawnRight = Random.Range(0, 2) == 0 ? ((camCurrPosTopRight.x < (MapGenerator.Instance.W - 1) * 32 - 32) ? true : false) : ((camCurrPosLowLeft.x > 32) ? false : true);

        do
        {
            if (spawnTopOrDown)
            {
                pos.y = spawnUp ? camCurrPosTopRight.y : camCurrPosLowLeft.y;
                pos.x = (int)Random.Range(camCurrPosLowLeft.x, camCurrPosTopRight.x);
            }
            else
            {
                pos.x = spawnRight ? camCurrPosTopRight.x : camCurrPosLowLeft.x;
                pos.y = (int)Random.Range(camCurrPosLowLeft.y, camCurrPosTopRight.y);
            }
        }
        while (IsWallAround(pos));

        pos.z = -1;

        return pos;
    }

    bool IsWallAround(Vector2 pos)
    {
        if (!MapGenerator.Instance.tm.isAccesible(new Vector2(pos.x, pos.y)) ||
           !MapGenerator.Instance.tm.isAccesible(new Vector2(pos.x, pos.y + 32)) ||
           !MapGenerator.Instance.tm.isAccesible(new Vector2(pos.x + 32, pos.y + 32)) ||
           !MapGenerator.Instance.tm.isAccesible(new Vector2(pos.x + 32, pos.y)) ||
           !MapGenerator.Instance.tm.isAccesible(new Vector2(pos.x + 32, pos.y - 32)) ||
           !MapGenerator.Instance.tm.isAccesible(new Vector2(pos.x, pos.y - 32)) ||
           !MapGenerator.Instance.tm.isAccesible(new Vector2(pos.x - 32, pos.y - 32)) ||
           !MapGenerator.Instance.tm.isAccesible(new Vector2(pos.x - 32, pos.y)) ||
           !MapGenerator.Instance.tm.isAccesible(new Vector2(pos.x - 32, pos.y + 32)))
            return true;

        return false;
    }

    public void UpdateGUI()
    {
        NoOfEnem.text = "Number of enemies: " + currNumOfEnemies.ToString();
        waveUI.text = "Current wave: " + currWave.ToString();
    }
}
