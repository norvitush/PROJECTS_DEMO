using System;
using UnityEngine;


//темплейт для сериализации из JSON - поля должны быть публичными

[Serializable]
public class Item
{
    public string Name;
    public int Id;
    public float Chance1_100;
    [Tooltip("Имя для поиска в префабах Resources")] public string PrefabName;
    public ItemAction ItemAction;
    public int Randomoffset;
    public Color32 PreViewColor;

    public Item() {}

    public Item(string name, int id, float chance, string prefabName, ItemAction it_action, int rand_offset, Color32 cl)
    {
        Name = name;
        Id = id;
        Chance1_100 = chance;
        PrefabName = prefabName;
        ItemAction = it_action;
        Randomoffset = rand_offset;
        PreViewColor = cl;
    }
    public override string  ToString()
    {
        return "Item "+ Id+ ", "+ Name;
    }
}

[Serializable]
public enum ItemAction
{
    CoinCollect = 0,
    ShieldRecieved = 1,
    SkinPartCollect = 2,
    none = 3,
    BonusCoinCollect = 4
}