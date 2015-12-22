using UnityEngine;

public enum ItemType {
    Consumable,
    Trowable,
    Collectable,
    Weapon
}

//Serves as class for collectable item
public class Item : MonoBehaviour {
    public int Id;
    public string Name;
    public string Description;
    public ItemType Type;
    public Sprite Sprite, SpriteHighlighted;
    public int MaxStack;

    //TODO: fix this when time comes!
    public override string ToString() {
        string color = "green";
        string newLine = "\n";
        return string.Format("<color="
            + color + "><size=16>{0}</size></color><size=14><i><color=lime>"
            + newLine + "{1}</color></i>{2}</size>", Name, Description, MaxStack);
    }
}
