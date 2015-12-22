using UnityEngine;
using UnityEngine.UI;

public enum MessageType {
    Quest,
    Hint,
    Info
}

// u pravu igru naasledi iz genericki singleton
public class MessageSystemController : MonoBehaviour {
    public static MessageSystemController Instance;

    private UIHandler _uiHandler;

    private MessageGiver _currentMessageGiver;
    public MessageGiver CurrentMessageGiver {
        get { return _currentMessageGiver; }
        set {
            _currentMessageGiver = value;
            if(_currentMessageGiver != null) {
                if(_currentMessageGiver.MessageType == MessageType.Hint)
                    ShowMessage();
                else _uiHandler.ShowButton(_currentMessageGiver.MessageType);
            } else {
                _uiHandler.CloseButton();
            }
        }
    }

    void Awake() {
        Instance = this;
        _uiHandler = UIHandler.Instance;
    }

    public void ShowMessage() {
        if(_currentMessageGiver == null || string.IsNullOrEmpty(_currentMessageGiver.Message)) return;
        MessageType msgType = _currentMessageGiver.MessageType;
        _uiHandler.ShowWindow(msgType, _currentMessageGiver.Message);
    }
}
