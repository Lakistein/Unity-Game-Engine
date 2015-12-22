using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
    public static UIHandler Instance;

    public GameObject 
        DialogButton,
        QuestWindow,
        HintWindow,
        InfoWindow,
        ItemAddedWindow;

    public Text QuestText, InfoText, HintText, ItemName, ButtonText;

    public Image ItemImage;

    private GameObject _activeWindow;

    void Awake() {
        Instance = this;
    }

    public void ShowButton(MessageType msgType) {
        switch(msgType) {
            case MessageType.Quest:
            case MessageType.Info:
                ButtonText.text = "Open";
                DialogButton.SetActive(true);
                break;
        }
    }

    public void CloseButton() {
        DialogButton.SetActive(false);
    }

    public void ShowWindow(MessageType msgType, string msg) {
        CloseWindow();
        switch(msgType) {
            case MessageType.Quest:
                QuestText.text = msg;
                QuestWindow.SetActive(true);
                _activeWindow = QuestWindow;
                PauseBackground();
                break;
            case MessageType.Info:
                InfoText.text = msg;
                InfoWindow.SetActive(true);
                _activeWindow = InfoWindow;
                PauseBackground();
                break;
            case MessageType.Hint:
                HintText.text = msg;
                HintWindow.SetActive(true);
                Invoke("CloseHintWindow", 3f);
                break;
        }
    }

    public void CloseHintWindow() {
        HintWindow.SetActive(false);
    }

    public void CloseWindow() {
        if(_activeWindow != null) {
            _activeWindow.SetActive(false);
            _activeWindow = null;
            ResumeBackground();
        }
    }

    public void ShowItemAddedWindow(Item item)
    {
        CancelInvoke("CloseItemAddedWindow");
        ItemName.text = item.Name;
        ItemImage.sprite = item.Sprite;
        ItemAddedWindow.SetActive(true);
        Invoke("CloseItemAddedWindow", 3f);
    }

    public void CloseItemAddedWindow()
    {
        ItemAddedWindow.SetActive(false);
    }

    void PauseBackground()
    {
        Time.timeScale = (float)0.0001;
    }

    void ResumeBackground()
    {
        Time.timeScale = 1;
    }
}
