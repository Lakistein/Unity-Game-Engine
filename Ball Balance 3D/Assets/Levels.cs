using UnityEngine;
using System.Collections;

public class Levels : MonoBehaviour
{

    public static byte llu = 20; 
    Camera cam;
    // Use this for initialization
    public static float SCR_WIDTH;
    public static float SCR_HEIGHT;

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        SCR_WIDTH = cam.pixelWidth;
        SCR_HEIGHT = cam.pixelHeight;
    }

    void OnGUI()
    {
        float rows = 5;
        float BUTTON_IN_ROW = 5;
        float PADDING = 5;
        float BUTTON_WIDTH = (cam.pixelWidth - PADDING) / BUTTON_IN_ROW - PADDING,
        BUTTON_HEIGHT = (cam.pixelHeight - PADDING) / rows - PADDING;

        Debug.Log("widrh " + BUTTON_WIDTH + ",  " + BUTTON_HEIGHT);

        int lvl = 0;
        for (int j = 0; j < rows; j++)
            for (int i = 0; i < BUTTON_IN_ROW; i++, lvl++)
            {
                if (lvl > llu) GUI.color = Color.gray;
                if (GUI.Button(new Rect(PADDING + i * (PADDING + BUTTON_WIDTH), PADDING + j * (PADDING + BUTTON_HEIGHT), BUTTON_WIDTH, BUTTON_HEIGHT), "" + (1 + lvl))&& lvl <= llu)
                {
                    MapGenerator.level = lvl;
                    Application.LoadLevel("Play");
                }
            }
    }
}
