using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object", menuName = "Inventory System/Items/Equipment")] //igual que antes, para no gastar tanto tiempo al crear nuevos objetos.
public class Equipment : ItemObject
{
    public float DPS; //Damage Per Second
    public float DR; //Damage Reduction
    public float TIER; //Rarity
    public void Awake()
    {
       type = ItemType.Equipment;
    }

}
