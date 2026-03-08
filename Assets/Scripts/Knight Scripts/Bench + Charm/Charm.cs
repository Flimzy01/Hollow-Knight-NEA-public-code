using UnityEngine;

[CreateAssetMenu(fileName = "NewCharm", menuName = "Charms/Charm")]
public class Charm : ScriptableObject
{
    //instantiates a class of everything that the charm prefabs will make use of
    public string charmName;
    public string description;
    public Sprite equippedSprite;
    public Sprite unequippedSprite;
    public int notchCost;
}