using UnityEngine;
using System.Collections;

public class PathApproach : State
{
    public float steerSpeed = 500;

    Vector3 desDir = new Vector3();
    Vector3 currDir = new Vector3();
    Vector3 move = new Vector3();

    Path lastPath;
    public override void Update(Entity mb)
    {
        Path p = MapGenerator.Instance.tm.
            findPath(mb.gameObject.transform.position.x + 16,
                      mb.gameObject.transform.position.y + 16,
                      Player.Instance.gameObject.transform.position.x,
                      Player.Instance.gameObject.transform.position.y
                      );
        if(p.next != null)
            p = p.next;

        if(lastPath != null)
            p = getPath(lastPath, p);
        lastPath = p;

        MapGenerator.Instance.debugPath = p;

        desDir.Set(p.x * 32 - mb.gameObject.transform.position.x,
                    p.y * 32 - mb.gameObject.transform.position.y, 0);
        desDir.Normalize();


        currDir.Set(-1, 0, 0);
        currDir = mb.gameObject.transform.rotation * currDir;
        currDir.Normalize();

        move = mb.gameObject.transform.position + desDir * Time.deltaTime * mb.Speed;
        move.z = -1;
        mb.moveOrSlide(move);

        if(lastPath != null && MapGenerator.Instance.tm.reachedTile(mb.gameObject.transform.position.x,
                                               mb.gameObject.transform.position.y,
                                                lastPath.x, lastPath.y))
            lastPath = lastPath.next;
    }
    private static Path getPath(Path last, Path _new)
    {
        Path p = last, p2 = _new;
        int c1 = 0, c2 = 0;
        while(p.next != null)
        {
            c1++;
            p = p.next;
        }
        while(p2.next != null)
        {
            c2++;
            p2 = p2.next;
        }
        if(p.x == p2.x && p.y == p2.y)
        {
            return last;
        }
        return _new;
    }
}
