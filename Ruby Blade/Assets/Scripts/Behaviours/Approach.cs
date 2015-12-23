using UnityEngine;
using System.Collections;
public class Approach : State
{
    public static Approach INSTANCE = new Approach();

    public float steerSpeed = 500;

    Vector3 desDir = new Vector3();
    Vector3 currDir = new Vector3();
    Vector3 move = new Vector3();

    public override void Update(Entity mb)
    {
        desDir.Set(Player.Instance.gameObject.transform.position.x - mb.gameObject.transform.position.x,
                    Player.Instance.gameObject.transform.position.y - mb.gameObject.transform.position.y, 0);
        currDir.Set(-1, 0, 0);
        currDir = mb.gameObject.transform.rotation * currDir;
        currDir.Normalize();

        currDir = Vector3.RotateTowards(currDir, desDir, Time.deltaTime * steerSpeed, 0);

        move = mb.gameObject.transform.position + currDir * Time.deltaTime * mb.Speed;
        move.z = -1;
        mb.moveOrSlide(move);
    }
}
