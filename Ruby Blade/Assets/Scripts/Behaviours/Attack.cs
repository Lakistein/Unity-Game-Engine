using UnityEngine;
using System.Collections;

public class Attack : State
{
    public static Attack INSTANCE = new Attack();

    public override void Update(Entity mb)
    {
        if (mb.AttackCooldown <= 0)
        {
            Player.Instance.TakeDemage(mb.DMG);
            mb.AttackCooldown = mb.attackSpeed;
        }
        else
            mb.AttackCooldown -= Time.deltaTime;
    }
}
