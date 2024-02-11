using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public enum ItemType //estos son los diferentes tipos de objetos
{
    Food,
    Equipment,
    Default
}

public enum Attributes //Estos son los atributos de los objetos. Gracias al metodo que se uso, podemos hacer que el mismo objeto tenga varias variantes.
{
    Dexterity,
    Intelligence,
    Constitution,
    Strength
    
}
public abstract class ItemObject : ScriptableObject
{   //clase es abstracta para usarla como base y extnder otros items a traves de ella.

    public int Id;
    public Sprite uiDisplay;
    public ItemType type; //esto es para poder checar el type.
    [TextArea(15, 20)] //esto hara facil leer las descripciones de los objetos.
    public string description;
    public ItemBuff[] buffs;

    public Item CreateItem() //esto es por si queremos crear un objeto con buffs.
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable] //esto es para que sea visible en el editor.
public class Item //Esto antes era ItemObject. Se cambio a Item para poder hacer clases de Items, que los hacen mas faciles de guardar y nos dejan hacer mas variantes.
{
    public string Name;
    public int Id;
    public ItemBuff[] buffs;
    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.Id;
        buffs = new ItemBuff[item.buffs.Length]; //esto se hace para evitar que toda la clase se regenere cuando llamas al dios de RNG
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.buffs[i].min, item.buffs[i].max);
            buffs[i].attribute = item.buffs[i].attribute;
        }

    }
}
[System.Serializable]
public class ItemBuff //estas son tablas de RNG
{
    public Attributes attribute;
    public int value;
    public int min;
    public int max;
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }
    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}