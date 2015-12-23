using UnityEngine;

public class Entity : MonoBehaviour
{
    public float maxHP;
    public float HP { set; get; }
    public int Speed;
    public int DMG;
    public float attackSpeed = 2;
    public int halfSize = 16;
    protected byte lastDir;
    protected Animator anim;
    public float stunnedFor = 0;


    public float AttackCooldown { get; set; }


    protected Camera cam;

    protected virtual void Start()
    {
        HP = maxHP;
        cam = Camera.main;
        anim = gameObject.GetComponent<Animator>();

    }

    public virtual void TakeDemage(int dmg)
    {
        HP -= dmg;
        UpdateGUI();
        if (HP <= 0)
            Die();
    }

    public virtual void AddHP(float hp)
    {
        HP += hp;
        if (HP > maxHP)
        {
            float temp = HP - maxHP;
            HP -= temp;
        }
        UpdateGUI();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected void LookAt(Vector2 posToLookAt)
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(posToLookAt.y, posToLookAt.x) * Mathf.Rad2Deg - 90.0f, Vector3.forward);
    }
    public void moveOrSlide(Vector3 m)
    {
        if (!move(m))
        {
            float x = m.x;
            m.x = gameObject.transform.position.x;
            if (!move(m))
            {
                m.x = x;
                m.y = gameObject.transform.position.y;
                move(m);
            }
        }
    }
    public virtual bool move(Vector3 move)
    {
        if (MapGenerator.Instance.tm.isAccesible(move.x + 16 - halfSize, move.y + 16 - halfSize) &&
            MapGenerator.Instance.tm.isAccesible(move.x + 16 + halfSize, move.y + 16 - halfSize) &&
            MapGenerator.Instance.tm.isAccesible(move.x + 16 - halfSize, move.y + 16 + halfSize) &&
            MapGenerator.Instance.tm.isAccesible(move.x + 16 + halfSize, move.y + 16 + halfSize))
        {
            Animate(transform.position, move);

            transform.position = move;
            return true;
        }
        return false;
    }

    void Animate(Vector3 pos, Vector3 newPos)
    {
        if (newPos.x > pos.x)
            lastDir = 1;
        else if (newPos.x < pos.x)
            lastDir = 3;
        else if (newPos.y > pos.y)
            lastDir = 0;
        else if (newPos.y < pos.y)
            lastDir = 2;
        else
            return;
        anim.SetInteger("Dir", lastDir);
    }
    protected virtual void UpdateGUI()
    { }
}
