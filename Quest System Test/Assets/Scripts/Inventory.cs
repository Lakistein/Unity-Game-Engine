using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text;

public class Inventory : MonoBehaviour {
    public static Inventory Instance;

    private RectTransform _inventoryRect;
    private List<GameObject> _slots;
    private float _inventoryWidth, _inventoryHeight;
    private static Slot _from, _to;
    private static GameObject _hoverObject;
    private float _hoverYOffset;
    private bool _fadingIn, _fadingOut;
    private static GameObject _tooltip;
    private static Text _visualText, _sizeText;

    public GameObject Tooltip, SlotContainer;
    public EventHandler ItemAdded;
    public int Slots, Rows;
    public float SlotPaddingLeft, SlotPaddingTop, SlotSize;
    public GameObject SlotPrefab, IconPrefab;
    public Canvas Canvas;
    public EventSystem EventSystem;
    public CanvasGroup CanvasGroup;
    public float FadeTime;
    public Text SizeText, VisualText;

    #region Items Prototypes
    public GameObject Tanzanite;
    #endregion

    public int EmptySlots { get; set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _tooltip = Tooltip;
        _visualText = VisualText;
        _sizeText = SizeText;
        CreateLayout();
        //LoadInventory();
    }

    private void Update() {
        if(Input.GetMouseButtonUp(0)) {
            if(!EventSystem.IsPointerOverGameObject(-1) && _from != null) {
                _from.GetComponent<Image>().color = Color.white;
                _from.ClearSlot();
                Destroy(_hoverObject);

                _from = null;
                _to = null;
                Destroy(_hoverObject);
                EmptySlots++;
            }
        }

        if(_hoverObject != null) {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.transform as RectTransform, Input.mousePosition, Canvas.worldCamera, out position);
            position.Set(position.x, position.y - _hoverYOffset);
            _hoverObject.transform.position = Canvas.transform.TransformPoint(position);
        }
        if(Input.GetKeyDown(KeyCode.I)) {
            if(CanvasGroup.alpha > 0) {
                StartCoroutine("FadeOut");
                PutItemBack();
            } else {
                StartCoroutine("FadeIn");
            }
        }
    }

    public void ShowTooltip(GameObject slot) {
        Slot tmpSlot = slot.GetComponent<Slot>();

        if(!tmpSlot.IsEmpty && _hoverObject == null) {
            _tooltip.SetActive(true);

            _visualText.text = tmpSlot.CurrentItem.ToString();
            _sizeText.text = _visualText.text;

            float xPos = slot.transform.position.x + SlotPaddingLeft;
            float yPos = slot.transform.position.y - slot.GetComponent<RectTransform>().sizeDelta.y - SlotPaddingTop;

            _tooltip.transform.position = new Vector2(xPos, yPos);
        }
    }

    public void HideTooltip() {
        _tooltip.SetActive(false);
    }

    private void OnItemAdded(Item item) {
        if(ItemAdded != null)
            ItemAdded(this, new ItemEventArgs() { Item = item });
        SaveInventory();
    }

    private void CreateLayout() {
        if(_slots != null) {
            foreach(var slot in _slots) {
                Destroy(slot);
            }
        }

        _slots = new List<GameObject>();

        _hoverYOffset = SlotSize * 0.01f;

        EmptySlots = Slots;

        _inventoryWidth = (Slots / Rows) * (SlotSize + SlotPaddingLeft) + SlotPaddingLeft;

        _inventoryHeight = Rows * (SlotSize + SlotPaddingTop) + SlotPaddingTop;

        _inventoryRect = GetComponent<RectTransform>();

        _inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _inventoryWidth);
        _inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inventoryHeight);

        int columns = Slots / Rows;

        for(int i = 0; i < Rows; i++) {
            for(int j = 0; j < columns; j++) {
                GameObject newSlot = Instantiate(SlotPrefab);
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();
                newSlot.name = "Slot";
                newSlot.transform.SetParent(SlotContainer.transform);
                slotRect.localPosition = _inventoryRect.localPosition + new Vector3(SlotPaddingLeft * (j + 1) + (SlotSize * j), -SlotPaddingTop * (i + 1) - (SlotSize * i));
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SlotSize * Canvas.scaleFactor);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SlotSize * Canvas.scaleFactor);
                _slots.Add(newSlot);
            }
        }
    }

    private bool PlaceEmpty(Item item) {
        if(EmptySlots > 0) {
            foreach(GameObject slot in _slots) {
                Slot tmp = slot.GetComponent<Slot>();
                if(tmp.IsEmpty) {
                    tmp.AddItem(item);
                    EmptySlots--;
                    OnItemAdded(item);
                    return true;
                }
            }
        }
        return false;
    }

    private void PutItemBack() {
        if(_from != null) {
            Destroy(_hoverObject);
            _from.GetComponent<Image>().color = Color.white;
            _from = null;
        }
    }

    public void SaveInventory() {
        StringBuilder sb = new StringBuilder();

        for(int i = 0; i < _slots.Count; i++) {
            Slot tmpSlot = _slots[i].GetComponent<Slot>();

            if(!tmpSlot.IsEmpty) {
                sb.Append(i + "-" + tmpSlot.CurrentItem.Type.ToString() + "-" + tmpSlot.Items.Count.ToString() + ";");
            }
        }

        PlayerPrefs.SetString("Content", sb.ToString());
        PlayerPrefs.SetInt("slots", Slots);
        PlayerPrefs.SetInt("rows", Rows);
        PlayerPrefs.SetFloat("slotPadingLeft", SlotPaddingLeft);
        PlayerPrefs.SetFloat("slotPadingTop", SlotPaddingTop);
        PlayerPrefs.SetFloat("slotSize", SlotSize);
        PlayerPrefs.SetFloat("xPos", _inventoryRect.position.x);
        PlayerPrefs.SetFloat("yPos", _inventoryRect.position.y);
        PlayerPrefs.Save();
    }

    public void LoadInventory() {
        string content = PlayerPrefs.GetString("Content");
        Slots = PlayerPrefs.GetInt("slots");
        Rows = PlayerPrefs.GetInt("rows");
        SlotPaddingLeft = PlayerPrefs.GetFloat("slotPadingLeft");
        SlotPaddingTop = PlayerPrefs.GetFloat("slotPadingTop");
        SlotSize = PlayerPrefs.GetFloat("slotSize");
        _inventoryRect.position = new Vector3(PlayerPrefs.GetFloat("xPos"), PlayerPrefs.GetFloat("yPos"));
        //CreateLayout();
        string[] splitContent = content.Split(';');

        for(int i = 0; i < splitContent.Length - 1; i++) {
            string[] splitValues = splitContent[i].Split('-');
            int index = int.Parse(splitValues[0]);
            // ItemType type = (ItemType)Enum.Parse(typeof(ItemType), splitValues[1]);
            int count = int.Parse(splitValues[2]);

            for(int j = 0; j < count; j++) {
                _slots[index].GetComponent<Slot>().AddItem(Tanzanite.GetComponent<Item>());
            }
        }
    }

    private IEnumerator FadeOut() {
        if(!_fadingOut) {
            _fadingOut = true;
            _fadingIn = false;
            StopCoroutine("FadeIn");

            CanvasGroup.interactable = false;

            float startAlpha = CanvasGroup.alpha;
            float rate = 1.0f / FadeTime;
            float progress = 0.0f;

            while(progress < 1.0) {
                CanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, progress);
                progress += rate * Time.deltaTime;
                yield return null;
            }
            CanvasGroup.alpha = 0;
            _fadingOut = false;
        }
    }

    private IEnumerator FadeIn() {
        if(!_fadingIn) {
            _fadingOut = false;
            _fadingIn = true;
            StopCoroutine("FadeOut");

            float startAlpha = CanvasGroup.alpha;
            float rate = 1.0f / FadeTime;
            float progress = 0.0f;

            while(progress < 1.0) {
                CanvasGroup.alpha = Mathf.Lerp(startAlpha, 1, progress);
                progress += rate * Time.deltaTime;
                yield return null;
            }
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            _fadingIn = false;
        }
    }

    public bool AddItem(Item item) {
        if(item.MaxStack == 1) {
            PlaceEmpty(item);
            return true;
        }
        foreach(GameObject slot in _slots) {
            Slot tmp = slot.GetComponent<Slot>();
            if(!tmp.IsEmpty) {
                //TODO: izmeni na kraj
                if(tmp.CurrentItem.Id == item.Id && tmp.IsAvaliable) {
                    tmp.AddItem(item);
                    OnItemAdded(item);
                    return true;
                }
            }
        }
        if(EmptySlots > 0)
            PlaceEmpty(item);
        return false;
    }

    public void MoveItem(GameObject clicked) {
        if(_from == null) {
            if(!clicked.GetComponent<Slot>().IsEmpty) {
                _from = clicked.GetComponent<Slot>();
                _from.GetComponent<Image>().color = Color.gray;

                _hoverObject = Instantiate(IconPrefab);
                _hoverObject.GetComponent<Image>().sprite = clicked.GetComponent<Image>().sprite;
                _hoverObject.name = "Hover";

                RectTransform hoverTransform = _hoverObject.GetComponent<RectTransform>();
                RectTransform clickedTransform = clicked.GetComponent<RectTransform>();

                hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clickedTransform.sizeDelta.x);
                hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clickedTransform.sizeDelta.y);

                //TODO: popravi gameobject.find
                _hoverObject.transform.SetParent(GameObject.Find("InventoryCanvas").transform, true);
                _hoverObject.transform.localScale = _from.gameObject.transform.localScale;
            }
        } else if(_to == null) {
            _to = clicked.GetComponent<Slot>();
            Destroy(_hoverObject);
        }
        if(_to != null && _from != null) {
            Stack<Item> tmpTo = new Stack<Item>(_to.Items);
            _to.AddItems(_from.Items);

            if(tmpTo.Count == 0) {
                _from.ClearSlot();
            } else {
                _from.AddItems(tmpTo);
            }
            _from.GetComponent<Image>().color = Color.white;
            _to = null;
            _from = null;
            Destroy(_hoverObject);
        }
    }
}