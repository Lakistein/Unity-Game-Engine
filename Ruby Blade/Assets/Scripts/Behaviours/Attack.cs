using UnityEngine;
using System.Collections;

public class Attack : State {

	public static Attack INSTANCE = new Attack();

//	IEnumerator DoDMG(byte amount)
//	{
//		yield return new WaitForSeconds (0.5f);
//		Player.Instance.TakeDemage (amount);
//	}

	public override void Update(Entity mb) {
		//Player.Instance.StartCoroutine (DoDMG(mb.DMG));
		if (mb.AttackCooldown <= 0) {
			Player.Instance.TakeDemage (mb.DMG);
			mb.AttackCooldown = mb.attackSpeed;
		} else
			mb.AttackCooldown -= Time.deltaTime; 
	}
}
