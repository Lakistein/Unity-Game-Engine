using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {
    public static PlayerStats Instance;

    public int Health;
    public int Energy;
    public int Experience;
    public int Money;
    public int Tranzanite;

    void Awake() {
        Instance = this;
    }

    private void SaveToDB() {
    }

    private void LoadFromDB() {
    }
}
