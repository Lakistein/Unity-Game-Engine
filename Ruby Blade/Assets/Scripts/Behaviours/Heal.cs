using UnityEngine;
using System.Collections;

public class Heal : State
{
    public static Heal INSTANCE = new Heal();

    public override void Update(Entity e)
    {
        e.HP += Time.deltaTime * 3;
        if (e.HP > e.maxHP)
            e.HP = e.maxHP;
    }
}
