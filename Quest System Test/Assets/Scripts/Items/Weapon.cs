using UnityEngine;
using System.Collections;

public class Weapon : Item
{
    public int Demage, Health, Energy, EnergyRegenerationPerSecond;

    //TODO: make this when time comes!
    public override string ToString() {
        string color = "green";
        string newLine = "\n";
        return string.Format("<color="
            + color + "><size=16>{0}</size></color><size=14><i><color=lime>"
            + newLine + "{1}</color></i>{2}</size>", Name, Description, MaxStack);
    }
}
