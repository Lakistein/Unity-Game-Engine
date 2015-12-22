using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler {
    public Text StackTxt;
    public Sprite SlotEmpty;
    public Sprite SlotHighlighted;

    public bool IsEmpty {
        get { return Items.Count == 0; }
    }

    public Item CurrentItem {
        get { return Items.Peek(); }
    }

    public bool IsAvaliable {
        get { return CurrentItem.MaxStack > Items.Count; }
    }

    public Stack<Item> Items { get; set; }

    void Awake() {
        Items = new Stack<Item>();
    }

    void Start() {
        RectTransform slotRect = GetComponent<RectTransform>();
        RectTransform txtRect = StackTxt.GetComponent<RectTransform>();

        int txtScaleFactor = (int)(slotRect.sizeDelta.x * 0.60);
        StackTxt.resizeTextMaxSize = txtScaleFactor;
        StackTxt.resizeTextMinSize = txtScaleFactor;

        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);
        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotRect.sizeDelta.y);
    }

    private void ChangeSprite(Sprite neutral, Sprite highlighted) {
        GetComponent<Image>().sprite = neutral;

        var st = new SpriteState {highlightedSprite = highlighted, pressedSprite = neutral};
        GetComponent<Button>().spriteState = st;
    }

    private void UpdateStackText() {
        StackTxt.text = Items.Count > 1 ? Items.Count.ToString() : string.Empty;
    }

    public void AddItem(Item item) {
        Items.Push(item);

        UpdateStackText();

        ChangeSprite(item.Sprite, item.SpriteHighlighted);
    }

    public void AddItems(Stack<Item> items) {
        this.Items = new Stack<Item>(items);

        ChangeSprite(CurrentItem.Sprite, CurrentItem.SpriteHighlighted);
        UpdateStackText();
    }

    public void ClearSlot() {
        Items.Clear();
        ChangeSprite(SlotEmpty, SlotHighlighted);
        UpdateStackText();
    }

    public void UseItem() {
        if(IsEmpty) return;
        if(!(Items.Peek() is IConsumable)) {
            Debug.Log("This item is not consumable");
            return;
        }

        ((IConsumable)Items.Pop()).Consume();

        UpdateStackText();
        if (!IsEmpty) return;
        ChangeSprite(SlotEmpty, SlotHighlighted);
        Inventory.Instance.EmptySlots++;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(Inventory.Instance.CanvasGroup.interactable && eventData.button == PointerEventData.InputButton.Right && !GameObject.Find("Hover")) {
            UseItem();
        }
    }
}
