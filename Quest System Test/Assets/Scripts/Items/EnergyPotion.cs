using UnityEngine;

public class EnergyPotion : Item, IConsumable {
    public int EnergyToRecover;

    public void Consume() {
        Debug.Log(EnergyToRecover + " energy recovered");
    }
}
