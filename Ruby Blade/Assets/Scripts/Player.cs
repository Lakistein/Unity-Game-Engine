using UnityEngine;
using System.Collections;

public class Player : Entity
{
    public static Player Instance;
    public Sprite[] sword;
    public GameObject SwordGO;
    private SpriteRenderer swordSR;
    private GameObject weapon;
    private RangedWeapon weaponScript;

    private bool hasWeapon = false;
    private bool isShieldOn = false;
    private bool swordSwinged = false;
    private int dmgAbs = 0;
    private int normalSpeed, SpeedUp;
    private float swordSpeed = 0.25f;
    private GUIText hpGUI;
    private SpriteRenderer shieldSprite;

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        transform.position = new Vector3(MapGenerator.Instance.W * 16f, MapGenerator.Instance.H * 16f, -1);
        hpGUI = GameObject.Find("HPUI").GetComponent<GUIText>();
        hpGUI.text = "HP: " + HP.ToString();
        normalSpeed = Speed;
        SpeedUp = Speed + 80;
        shieldSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        swordSR = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Move();
        Sword();
        if (weapon != null && Input.GetMouseButton(0))
            weaponScript.Fire();

    }
    void Sword()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !swordSwinged)
        {
            switch (lastDir)
            {
                case 0: swordSR.sprite = sword[0]; break;
                case 1: swordSR.sprite = sword[1]; break;
                case 2: swordSR.sprite = sword[2]; break;
                case 3: swordSR.sprite = sword[3]; break;
            }
            swordSwinged = true;
            SwordDmg();
        }

        SwordFade();
    }

    void SwordFade()
    {
        Color c = SwordGO.renderer.material.color;
        if (swordSwinged && swordSpeed > 0)
        {
            swordSpeed -= Time.deltaTime;
            c.a = swordSpeed * 4;
            SwordGO.renderer.material.color = c;
        }
        else
        {
            swordSpeed = 0.25f;
            swordSwinged = false;
        }
    }

    void SwordDmg()
    {
        float x = gameObject.transform.position.x;
        float y = gameObject.transform.position.y;
        switch (lastDir)
        {
            case 0: y += 24; break;
            case 1: x += 16; break;
            case 2: y -= 24; break;
            case 3: x -= 16; break;
        }
        Vector2 vv = new Vector2(x, y);
        //move x & y based on player dir
        Vector2 aa = new Vector2();

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Vector2.Distance(enemy.transform.position, vv) < 48)
            {
                enemy.SendMessage("TakeDemage", 10, SendMessageOptions.DontRequireReceiver);
                aa.Set(enemy.transform.position.x - gameObject.transform.position.x,
                       enemy.transform.position.y - gameObject.transform.position.y);
                aa.Normalize();
                enemy.transform.position = new Vector3(enemy.transform.position.x + aa.x * 32,
                                                        enemy.transform.position.y + aa.y * 32, -1);

            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Weapon"))
        {
            PickWeapon(col.gameObject);
        }

        if (col.gameObject.name.Contains("MagazinePowerUp") && hasWeapon)
        {
            PickMagazine();
            Destroy(col.gameObject);
        }
    }

    void PickWeapon(GameObject go)
    {
        if (hasWeapon)
        {
            if (go.name.Contains(weapon.name))
            {
                weaponScript.magazineCount += 3;
                if (weaponScript.magazineCount == 3)
                    weaponScript.Reload();
                Destroy(go);
                return;
            }
        }

        weapon = go.gameObject;
        go.transform.parent = transform;
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;

        weaponScript = weapon.GetComponent<RangedWeapon>();
        weaponScript.UpdateGUI();
        weapon.GetComponent<SpriteRenderer>().enabled = false;
        weapon.GetComponent<BoxCollider2D>().enabled = false;
        hasWeapon = true;
    }

    void PickMagazine()
    {
        weaponScript.AddMagazine();
    }

    private IEnumerator IncreaseS(float time)
    {
        Speed = SpeedUp;
        yield return new WaitForSeconds(time);
        Speed = normalSpeed;
    }

    public void IncreaseSpeed(float time)
    {
        StopCoroutine("IncreaseS");
        StartCoroutine(IncreaseS(time));
    }

    private IEnumerator ActivateS(float time)
    {
        isShieldOn = true;
        yield return new WaitForSeconds(time);
        isShieldOn = false;
        shieldSprite.enabled = false;
    }

    //time, dmgAbs
    public void ActivateShield(float[] parameters)
    {
        float time = parameters[0];
        dmgAbs = (int)parameters[1];
        shieldSprite.enabled = true;
        StopCoroutine("ActivateS");
        StartCoroutine(ActivateS(time));
    }

    public override void TakeDemage(int dmg)
    {
        if (isShieldOn)
        {
            dmgAbs -= dmg;
            if (dmgAbs <= 0)
            {
                isShieldOn = false;
                shieldSprite.enabled = false;
                StopCoroutine("ActivateS");
            }
            return;
        }

        base.TakeDemage(dmg);

        if (HP >= 0)
            hpGUI.text = "HP: " + HP.ToString();
        else
            hpGUI.text = "HP: 0";

    }

    protected override void Die()
    {
        HP = maxHP;
        transform.position = new Vector3(MapGenerator.Instance.W * 16f, MapGenerator.Instance.H * 16f, -1);
    }

    void Move()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        anim.speed = (move.x == 0 && move.y == 0) ? 0 : 1;

        move = transform.position + move * Speed * Time.deltaTime;

        base.moveOrSlide(move);
    }

    public static Quaternion LookAt(Vector2 posToLookAt)
    {
        return Quaternion.AngleAxis(Mathf.Atan2(posToLookAt.y, posToLookAt.x) * Mathf.Rad2Deg - 90.0f, Vector3.forward);
    }

    protected override void UpdateGUI()
    {
        hpGUI.text = "HP: " + HP.ToString();
    }
}
