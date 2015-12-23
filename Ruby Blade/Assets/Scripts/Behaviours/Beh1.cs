using UnityEngine;
using System.Collections;

public class Beh1 : AIBehaviour
{
    public float distance = 1;
    public override void UpdateBehaviour(AIBehaviour mb, Entity e)
    {
        if (e.stunnedFor > 0)
        {
            e.stunnedFor -= Time.deltaTime;
            return;
        }
        if (distance < Vector2.Distance(Player.Instance.gameObject.transform.position, mb.gameObject.transform.position))
        {
            mb.Current = Approach.INSTANCE;
        }
        else
        {
            mb.Current = Attack.INSTANCE;
        }
    }

}
