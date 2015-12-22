using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(MessageGiver))]
public class QuestGiver : MonoBehaviour {
    private GameObject _yellowDiamond, _greenDiamond;
    private QuestManager _questManager;
    private MessageGiver _messageGiver;
    private bool _hasQuest, _hasQuestToAccomplish;
    private int _questToAccomplishId;

    public int Id;
    public List<Quest> Quests;

    void Awake() {
        if((_questManager = QuestManager.Instance) == null)
            _questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>();
        _messageGiver = GetComponent<MessageGiver>();

        _yellowDiamond = transform.FindChild("YellowDiamond").gameObject;
        _greenDiamond = transform.FindChild("GreenDiamond").gameObject;
    }

    void OnEnable() {
        //_questManager.QuestAccepted += OnQuestAccepted;
        _questManager.QuestCompleted += OnQuestCompleted;
        //_questManager.QuestAccomplished += OnQuestAccomplished;
        _questManager.QuestAvaliable += OnQuestAvaliable;

        ActivateQuestIfAnyAvaliable();
    }

    void OnDisable() {
        //if(_questManager.QuestAccepted != null) _questManager.QuestAccepted -= OnQuestAccepted;
        if(_questManager.QuestCompleted != null) _questManager.QuestCompleted -= OnQuestCompleted;
        //if(_questManager.QuestAccomplished != null) _questManager.QuestAccomplished -= OnQuestAccomplished;
        if(_questManager.QuestAvaliable != null) _questManager.QuestAvaliable -= OnQuestAvaliable;
    }

    #region Diamonds

    private void DisableYellowDiamond() {
        _yellowDiamond.SetActive(false);
    }

    private void EnableYellowDiamond() {
        _greenDiamond.SetActive(false);
        _yellowDiamond.SetActive(true);
    }

    private void DisableGreenDiamond() {
        _greenDiamond.SetActive(false);
    }

    private void EnableGreenDiamond() {
        _yellowDiamond.SetActive(false);
        _greenDiamond.SetActive(true);
    }

    #endregion

    private void GiveTextToMessageGiver(string text) {
        if(!string.IsNullOrEmpty(text))
            _messageGiver.Message = text;
    }

    private void ActivateQuestIfAnyAvaliable() {
        if(_hasQuestToAccomplish) return;

        if(_questManager.IsQuestAccomplishers(this.Id, out _questToAccomplishId)) {
            ActivateQuestToAccomplish(_questToAccomplishId);
            return;
        }

        for(int i = 0; i < Quests.Count; i++) {
            if(Quests[i].Status != QuestStatus.Eligible) continue;
            _hasQuest = true;
            EnableYellowDiamond();
            GiveTextToMessageGiver(Quests[i].Description);
            return;
        }
    }

    private void ActivateQuestToAccomplish(int id) {
        _hasQuestToAccomplish = true;
        EnableGreenDiamond();
        _questToAccomplishId = id;
        GiveTextToMessageGiver(_questManager.GetQuest(id).Congratulations);
    }

    #region EventHandlers
    //TODO: on enter, make UI (dialogs), maybe setup camera etc
    void OnTriggerEnter(Collider col) {
        if(col.gameObject.CompareTag("Player")) {
            if(_hasQuestToAccomplish) {
                _questManager.CurrentQuestGiver = this;
                _questManager.AccomplishQuest(_questToAccomplishId); // just for testing
                _hasQuestToAccomplish = false;
                DisableGreenDiamond();
                ActivateQuestIfAnyAvaliable();
            } else if(_hasQuest) {
                _questManager.CurrentQuestGiver = this;
                _questManager.AcceptQuest();
                _hasQuest = false;
                ActivateQuestIfAnyAvaliable();
            }
        }
    }

    void OnTriggerExit(Collider col) {
        if(col.gameObject.CompareTag("Player")) {
            if(_hasQuestToAccomplish || _hasQuest)
                _questManager.CurrentQuestGiver = null;
        }
    }

    void OnQuestAccepted(object sender, QuestEventArgs quest) {
        if(Quests.Count == 0)
            return;
        /*
        if(Quests.Contains(quest.Quest)) {
            Quests.Remove(quest.Quest);
            DisableYellowDiamond();
        }*/
    }

    void OnQuestCompleted(object sender, QuestEventArgs quest) {
        if(!_hasQuestToAccomplish && quest.Quest.AccomplisherId == this.Id) {
            ActivateQuestToAccomplish(quest.Quest.Id);
        }
    }

    /* void OnQuestAccomplished(object sender, QuestEventArgs quest) {
         ActivateQuestIfAnyAvaliable();
     }*/

    void OnQuestAvaliable(object sender, QuestEventArgs quest) {
        if(Quests.Count > 0)
            ActivateQuestIfAnyAvaliable();
    }
    #endregion

    public Quest GetEligibleQuest() {
        Quest quest = Quests.Find(q => q.Status == QuestStatus.Eligible);
        if(quest != null) {
            Quests.Remove(quest);
            DisableYellowDiamond();
        }
        return quest;
    }
}
