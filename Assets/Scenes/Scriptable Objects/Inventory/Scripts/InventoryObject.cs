using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using JetBrains.Annotations;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")] //Con esto, cada vez que creemos un nuevo objeto para un nuevo GameObject, se creara en el inventario.
//Se esta creando un inventario como objeto para poder tener varios diferentes inventarios, ie, una caja, un cofre, no solo el del jugador.
public class InventoryObject : ScriptableObject  //ISerializationCallbackReceiver // Sin Json, sin necesidad.
{
    public string savePath; //esto nos dejara guardar inventario en el .json, pero tambien nos dejara tener varios inventarios.
    public ItemDatabaseObject database; //Esto nos deja guardar en el primer lugar. Originalmente era publica, pero al cambiarla a privada hacemos que la serializacion del dios Cruel de Unity no llegue aqui.
    public Inventory Container;

    public void AddItem(Item _item, int _amount) //Esto nos dejara añadir objetos a la lista.
    {

        if (_item.buffs.Length > 0)
        {
            SetEmptySlot(_item, _amount);
            //Container.Items.Add(new InventorySlot(_item.Id, _item, _amount)); Originalmente, teniamos un sistema que hacia nuevos Slots directamente en el menu. Pero con el nuevo array, ya no se necesita.
            return; //con esto, si el objeto aparece con un buff que no esta en la lista concurrente, contara como un item nuevo en la UI.
        }

        for (int i = 0; i < Container.Items.Length; i++) //esto es para revisar que el objeto no este repetido en la lista.
        {
            if (Container.Items[i].ID == _item.Id)
            {   
                Container.Items[i].AddAmount(_amount);
                return; //esto repite el codigo hasta encontrar un objeto que no exista en el inventario.
            }
        }
        SetEmptySlot(_item, _amount);

        //Container.Items.Add(new InventorySlot(_item.Id, _item, _amount)); //esto nos deja crear un nuevo objeto de inventario en el menu. 
        //Al Convertir de lista a array, tenemos que rehacer este codigo.


    }

    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0;i < Container.Items.Length; i++)
        {
            if (Container.Items[i].ID <= -1) //esto es una precaucion, por si algo se rompe y trata de dar un valor --.
            {
                Container.Items[i].UpdateSlot(_item.Id, _item, _amount);
                return Container.Items[i]; //esto es por si necesitamos hacer mas con el objeto, nos da una referencia.
            }
        }
        //esto es por si necesitamos trabajar con el proyecto en un futuro.
        return null;
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.ID, item2.item, item2.amount);
        item2.UpdateSlot(item1.ID, item1.item, item1.amount);
        item1.UpdateSlot(temp.ID, temp.item, temp.amount); //esto ayuda a los items a moverse en el inventario.
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot(-1, null, 0);
            }
        }
    } //Esto nos deja quitar items de nuestro inventario.


    [ContextMenu("Save")]
    public void Save()
    {
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));//El Concat sirve para combinar varios strings juntos, y asi ahorrar memoria. el persistentPath es una
        //funcion de Unity que te deja guardar datos a traves de plataformas.
        //bf.Serialize(file, saveData);
        //file.Close();   //Originalmente todo esto se usaba para la primera version del sistema de guardado. Pero ahora con el IFormatter y con el uso de ItemGround como su propia clase, ya no es necesario.

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close(); //Muy importante hacer esto para evitar fugas de memoria.
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
            
            {

            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this); //todo esto es para que podamos guardar.
            //file.Close(); //esto es para evitar memory leaks.  //Lo mismo que arriba, ya no necesitamos este super codigo.

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < Container.Items.Length; i++)
            {
                Container.Items[i].UpdateSlot(newContainer.Items[i].ID, newContainer.Items[i].item, newContainer.Items[i].amount);
            }
            stream.Close(); //IMPORTANTE. 

        }

        
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }





    //public void OnAfterDeserialize()
    //{
    //    for (int i = 0; i < Container.Items.Count; i++)
    //    {
    //        Container.Items[i].item = database.GetItem[Container.Items[i].ID];
    //    }
    //}

    //public void OnBeforeSerialize()
    //{
    //} Ya no es necesario esto sin el metodo de JSon
}


[System.Serializable]

public class Inventory //esto se convierte en una clase para poder sacar toda la informacion directamente del slot del inventario, y no del objeto directamente.
{ 
    public InventorySlot[] Items = new InventorySlot[24]; //Esto nos deja empezar a crear la lista/menu. Originalmente, esto era una List. Sin embargo, la cambiamos a un array para poder tener un inventario estatico
    //Y asi poder hacer slots manipulables.
    //para crear un menu, esto esta bien, pero para hacer que funcione mejor el Inventario, sera mejor hacer una clase que actue como el Slot.
}

[System.Serializable] //Esto have que, cuando agreguemos esta clase a un objeto, lo serializara y hara aparecer en el editor.
public class InventorySlot
{
    public int ID = -1;
    public Item item;
    public int amount;
    public InventorySlot() //Esta copia es para hacer que Unity coopere y no ponga todo como ID 0 en el inventario.
    {
        ID = -1;
        item = null;
        amount = 0;
    }

    public InventorySlot(int _id, Item _item, int _amount) //este actua como nuestro Slot, efectivamente es como un objeto que contiene otros objetos. Es el constructor.
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }

    public void UpdateSlot(int _id, Item _item, int _amount) //esto es para hacer un Update a la lista en vivo, bajo el nuevo sistema de Array.
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }

        public void AddAmount(int value)  //esta funcion nos ayudara a añadir objetos
    {
        amount += value;
    }
}