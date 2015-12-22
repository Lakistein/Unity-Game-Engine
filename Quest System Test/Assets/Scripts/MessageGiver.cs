using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MessageGiver : MonoBehaviour {
    [TextArea]
    public string Message;

    public MessageType MessageType;

    void OnTriggerEnter(Collider col) {
        if(col.gameObject.CompareTag("Player") && !string.IsNullOrEmpty(Message)) {
            MessageSystemController.Instance.CurrentMessageGiver = this;
            if (MessageType == MessageType.Hint)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit(Collider col) {
        if(col.gameObject.CompareTag("Player") && !string.IsNullOrEmpty(Message)) {
            MessageSystemController.Instance.CurrentMessageGiver = null;
        }
    }
}
