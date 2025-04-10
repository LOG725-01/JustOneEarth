using System.Collections.Generic;
using UnityEngine;

public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public Dictionary<RessourceTypes, int> cost = new Dictionary<RessourceTypes, int>();
    public List<ICardEffect> effectList = new List<ICardEffect>();
    public List<ICardCondition> conditionList = new List<ICardCondition>();
    public bool addOwnedTile = true;
    public bool isPersistent = false;
    public enum CardTargetType
    {
        NeutralTileOnly,
        EnemyTileOnly,
        Any
    }
    
    public CardTargetType targetType = CardTargetType.Any;

    public void InjectDefaultConditionIfNeeded()
    {
        switch (targetType)
        {
            case CardTargetType.NeutralTileOnly:
                conditionList.Add(new OnlyOnNeutralTile());
                break;
            case CardTargetType.EnemyTileOnly:
                conditionList.Add(new OnlyOnEnemyTile());
                break;
        }
    }
}