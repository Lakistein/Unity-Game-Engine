using UnityEngine;
using System.Collections;

public class Shoot : State {
	public static Shoot INSTANCE = new Shoot();

	public override void Update(Entity mb) {
		//Player.Instance.StartCoroutine (DoDMG(mb.DMG));
		if (mb.AttackCooldown <= 0) {
			BulletManager.Instance.SpawnBullet(true,
				   mb.gameObject.transform.position, 			
				   Player.LookAt(Player.Instance.gameObject.transform.position - mb.gameObject.transform.position));
			mb.AttackCooldown = mb.attackSpeed;
		} else
			mb.AttackCooldown -= Time.deltaTime; 
	}

}