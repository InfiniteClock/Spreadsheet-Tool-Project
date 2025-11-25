using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CropDataJSON
{
    public string itemName;
    public int id;
    public List<Sprite> spritelist;
    public int growthStages;
    public Color color;
    public int growthDays;
}
