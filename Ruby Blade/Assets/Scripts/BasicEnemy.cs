using UnityEngine;
using System.Collections;

public class BasicEnemy : Entity
{
    const int powerUpDropChance = 10;
    protected override void Start()
    {
        base.Start();
    }

    protected override void Die()
    {
        Spawner.Instace.currNumOfEnemies--;
        Spawner.Instace.UpdateGUI();
        if(Random.Range(0, powerUpDropChance) == 0)
            Spawner.Instace.SpawnPowerUp(transform.position);
        base.Die();
    }

	public override void TakeDemage (int dmg)
	{
		base.TakeDemage (dmg); 
		stunnedFor = 0.5f;
	}
}
