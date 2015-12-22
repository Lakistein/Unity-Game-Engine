using UnityEngine;
using UnityEngine.Serialization;

public class Quest: MonoBehaviour
{
    public int Id;
    public string Title;
    public QuestStatus Status;
    public QuestObjective Objective;
    [TextArea] public string Description;
    [TextArea] public string Hint;
    [TextArea] public string Congratulations;
    [TextArea] public string Summary;
    public int Experience;
    public Item[] Reward;
    public int AccomplisherId;
    public int NextQuestId;

    public override string ToString()
    {
        //TODO: to display quest data in some format
        return base.ToString();
    }
}

public enum QuestStatus {
    NotEligible,
    Eligible,
    Accepted,
    Completed,
    Accomplished
}

public enum QuestObjective
{
    TalkTo,
    Kill,
    Collect,
    Explore
}