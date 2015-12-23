using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour
{
    float xMin, xMax, yMin, yMax;

    void Awake()
    {
        camera.orthographicSize = Screen.height / 2;
    }
    void Start()
    {
        xMin = Camera.main.pixelWidth * 0.5f - 16;
        xMax = MapGenerator.Instance.tm.w * 32 - xMin - 32;
        yMin = Camera.main.pixelHeight * 0.5f - 16;
        yMax = MapGenerator.Instance.tm.h * 32 - yMin - 32;
    }

    void Update()
    {
        float x = Player.Instance.gameObject.transform.position.x,
        y = Player.Instance.gameObject.transform.position.y;

        if (x < xMin) x = xMin;
        else if (x > xMax) x = xMax;
        if (y < yMin) y = yMin;
        else if (y > yMax) y = yMax;
        transform.position = new Vector3(x, y, -10);
    }
}