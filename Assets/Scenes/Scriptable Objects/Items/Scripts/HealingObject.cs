using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Object", menuName = "Inventory System/Items/Food")] //Igual que en DefaultObject, esto nos deja clasificar todo automaticamente.
public class FoodObject : ItemObject //esta clase es para todos los objetos que restauran salud.
{

    public int restoreHPValue; //valor para saber cuanta HP restaurar.
    public int restoreMPValue; //valor para saber cuanta MP restaurar.
    public void Awake()
    {
        type = ItemType.Food;
    }
}
