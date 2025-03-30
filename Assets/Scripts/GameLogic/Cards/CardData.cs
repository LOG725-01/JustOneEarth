
using System.Collections.Generic;
using UnityEngine;

public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public Dictionary<RessourceTypes, int> cost = new Dictionary<RessourceTypes, int>();
    public List<ICardEffect> effectList = new List<ICardEffect>();
}