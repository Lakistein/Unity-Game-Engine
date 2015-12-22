using UnityEngine;
using System.Collections;
using System;

public class HealthGem : Item, IConsumable
{
    public int HealthToRestor;

    public void Consume() {
        Debug.Log(HealthToRestor + " health recovered");
    }
}
