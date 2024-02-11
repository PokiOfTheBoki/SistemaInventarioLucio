using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Default")]

public class DefaultObject : ItemObject  //Esto indica que es una clase derivada.
{
    public void Awake() //esto nos deja poner variables cada vez que creemos un nuevo objeto.
    {
        type = ItemType.Default; //Con esto ya no tenemos que poner un type cada vez que hagamos un nuevo objeto.
    }

}
