using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Crop", menuName = "Custom/Crop")]
public class Crop : ScriptableObject
{
    public string itemName;
    public int id;
    public Sprite sprite;
    public int growthStages;
    public Color color;
    public int growthDays;
}
