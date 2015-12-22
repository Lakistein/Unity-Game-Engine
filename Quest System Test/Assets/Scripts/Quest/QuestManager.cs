using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Debug = UnityEngine.Debug;

public class QuestManager : MonoBehaviour {
    public static QuestManager Instance;

    private static Dictionary<int, Quest> _quests;
    private List<int> _activeQuestsId, _completedQuestsId;
    private Inventory _inventory;

    public List<Quest> Quests;
    public QuestGiver CurrentQuestGiver { get; set; }

    public EventHandler<QuestEventArgs> QuestAccepted,
                                        QuestCompleted,
                                        QuestAccomplished,
                                        QuestAvaliable;

    void Awake() {
        Instance = this;
        _quests = new Dictionary<int, Quest>();
        _activeQuestsId = new List<int>();
        _completedQuestsId = new List<int>();

        foreach(Quest quest in Quests)
            _quests.Add(quest.Id, quest);

        Quests = null;
        if((_inventory = Inventory.Instance) == null) _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();

        PlayerPrefs.SetInt("asdasd", 10);
        PlayerPrefs.SetFloat("1213", 1.5f);
        PlayerPrefs.Save();
    }

    public Quest GetQuest(int id) {
        return _quests[id];
    }

    public void AcceptQuest() {
        if (CurrentQuestGiver == null) return;
        Quest q = CurrentQuestGiver.GetEligibleQuest();

        if (q == null) return;
        _activeQuestsId.Add(q.Id);
        q.Status = QuestStatus.Accepted;
        OnQuestAccepted(q);
        CompleteQuest(q.Id);
    }

    public void CompleteQuest(int id) {
        _activeQuestsId.Remove(id);
        _completedQuestsId.Add(id);
        _quests[id].Status = QuestStatus.Completed;
        OnQuestCompleted(_quests[id]);
    }

    public void AccomplishQuest(int id) {
        if(!_completedQuestsId.Contains(id))
            return;

        _completedQuestsId.Remove(id);
        _quests[id].Status = QuestStatus.Accomplished;

        for (int i = 0; i < _quests[id].Reward.Length; i++)
        {
            _inventory.AddItem(_quests[id].Reward[i]);
        }

        //TODO: add experience
        Debug.Log(_quests[id].Experience + " experience received");

        OnQuestAccomplished(_quests[id]);

        int nextQuestId = _quests[id].NextQuestId;
        if(nextQuestId == 0 || _quests[nextQuestId].Status != QuestStatus.NotEligible) return;

        _quests[nextQuestId].Status = QuestStatus.Eligible;
        OnQuestAvaliable(_quests[nextQuestId]);
    }

    public bool IsQuestActive(int id) {
        return _activeQuestsId.Contains(id);
    }

    public bool IsQuestCompleted(int id) {
        return _completedQuestsId.Contains(id);
    }

    public bool IsQuestAccomplishers(int accomplisherId, out int accomplishedQuestId) {
        for(int i = 0; i < _completedQuestsId.Count; i++) {
            if (_quests[_completedQuestsId[i]].AccomplisherId != accomplisherId) continue;
            accomplishedQuestId = _completedQuestsId[i];
            return true;
        }
        accomplishedQuestId = 0;
        return false;
    }

    #region EventHandlers
    private void OnQuestAccepted(Quest q) {
        if(QuestAccepted != null)
            QuestAccepted(this, new QuestEventArgs() { Quest = q });
    }

    private void OnQuestCompleted(Quest q) {
        if(QuestCompleted != null)
            QuestCompleted(this, new QuestEventArgs() { Quest = q });
    }

    private void OnQuestAccomplished(Quest q) {
        if(QuestAccomplished != null)
            QuestAccomplished(this, new QuestEventArgs() { Quest = q });
    }

    private void OnQuestAvaliable(Quest q) {
        if(QuestAvaliable != null)
            QuestAvaliable(this, new QuestEventArgs() { Quest = q });
    }
    #endregion
}
