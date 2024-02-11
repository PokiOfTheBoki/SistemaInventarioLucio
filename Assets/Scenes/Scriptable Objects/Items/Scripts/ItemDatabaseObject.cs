using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver //esto es para no tener que volver a hacer la Database cada vez que hagamos una nueva escena.
{
    public ItemObject[] Items;
    //public Dictionary<ItemObject, int> GetId = new Dictionary<ItemObject, int>(); Esto se uso inicialmente, pero no fue necesario al final.
    public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>(); //esto es para recuperar los objetos durante serializacion.

    public void OnAfterDeserialize()
    {
       // GetId = new Dictionary<ItemObject, int>(); //esto es para que el diccionario no este haciendo la misma cosa entre escenas.
        GetItem = new Dictionary<int, ItemObject>();
        for (int i = 0; i < Items.Length; i++)
        {
            //GetId.Add(Items[i], i); //con esto, cuando Unity serialize los objetos, inmediatemente se actualizara el diccionario.
            Items[i].Id = i; //Con esto los items siempre estaran puestos para serializar.
            GetItem.Add(i, Items[i]);
        }
    }

    //estas dos ^^ ,, son para que Unity haga su trabajo y serialize los objetos correctamente.
    public void OnBeforeSerialize()
    {
        GetItem  = new Dictionary<int, ItemObject>(); //Para mejor performance.
    }
}
