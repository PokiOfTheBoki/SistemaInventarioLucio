using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //esto es para denotar que estamos usando el TMP para la funcion Display
using UnityEngine.UI;
using UnityEditor.EventSystems;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DisplayInventory : MonoBehaviour  //esta Script es para actualizar y mostrar el inventario correctamente.
{
    public MouseItem mouseItem = new MouseItem();
    public GameObject inventoryPrefab;
    public InventoryObject inventory; //como estamos usando un Scriptable Object, podemos hacer un link directo al inventario del jugador, y no al jugador mismo.
    public int X_START;
    public int Y_START; //esto es para que los items empiecen en la lista desde la parte superior izquierda.
    public int X_SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;
    public int Y_SPACE_BETWEEN_ITEMS; //todo esto es para manejar el espacio entre objetos.
    Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>(); //esto originalmente era Inventory Slot primero, pero lo cambie para mejor agarrar datos para el inventario.

    private void Start()
    {
        CreateSlots();
    }

    private void Update()
    {
        UpdateSlots();
        //UpdateDisplay(); Con Array ya no es necesario.
    }

    public void UpdateSlots()
    { 
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemsDisplayed)
        {
            if(_slot.Value.ID >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[_slot.Value.item.Id].uiDisplay; //Esta cadena de pesadilla es para que Unity coopere. 
                //si no hacemos esto, el proyecto guardara el Display de la UI, no solo los datos dentro del inventario. Asi hay mejor performance, por que las partes graficas del item siguen en la database.
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1); //esto es para hacer que los espacios vacios no regresen como placas blancas.
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0"); //Esto es para que las cantidades funcionen en el display.
            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0); 
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = " ";  //esto es para no mostrar nada.
            }
        }
    }

    //public void UpdateDisplay()
    //{

        
    //    for (int i = 0; i < inventory.Container.Items.Count; i++)
    //    {
    //        InventorySlot slot = inventory.Container.Items[i]; //esto limpia el codigo.

    //        if (itemsDisplayed.ContainsKey(inventory.Container.Items[i]))
    //        {
    //            itemsDisplayed[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0"); //esto nos deja agregar objetos que ya tengamos al inventario.

    //        }
    //        else
    //        {
    //            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
    //            obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiDisplay;
    //            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
    //            obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0"); //esto nos deja agregar objetos que no tengamos.
    //            itemsDisplayed.Add(inventory.Container.Items[i], obj);  //esto nos deja añadirlo al Diccionario.
    //        }
    //    } Con Slots esto ya no nos sirve.

        
    //}
    public void CreateSlots() //esto hace que aparezcan los items que tengamos en el menu del Inventario. Esto solia ser Create Display.
    {

        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            itemsDisplayed.Add(obj, inventory.Container.Items[i]);
        }

        //for (int i = 0; i < inventory.Container.Items.Count; i++)
        //{
        //    InventorySlot slot = inventory.Container.Items[i]; //de cuatro palabras a solo una.

        //    var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform); //esto nos da la posicion para el Display.
        //    obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiDisplay;
        //    obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
        //    obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0"); //Esto es para formatearlo con comas, para que se vea mejor.
        //    itemsDisplayed.Add(slot, obj);
        //} Igual que lo de List, esto se tiene que quitar para convertir de List a Array y asi tener un inventario mas sencillo de hacer.
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action) //esto es para que los botones funcionen.
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger); //esto parece complicado, pero nada mas es para no tener que hacer todo esto cuando queramos hacer un nuevo evento.
    }

    public void OnEnter(GameObject obj)
    {
        mouseItem.hoverObj = obj;
        if(itemsDisplayed.ContainsKey(obj))
            mouseItem.hoverItem = itemsDisplayed[obj]; //esto es para que reconozca que movemos un objeto.
        
    }
    public void OnExit(GameObject obj)
    {
        mouseItem.hoverObj = null;
        mouseItem.hoverItem = null; //esto es para que reconozca que soltamos un objeto.
    }
    public void OnDragStart(GameObject obj)
    {
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        mouseObject.transform.SetParent(transform.parent);
        if (itemsDisplayed[obj].ID >= 0)
        {
            var img = mouseObject.AddComponent<Image>();
            img.sprite = inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay;
            img.raycastTarget = false;
        }
        mouseItem.obj = mouseObject;
        mouseItem.item = itemsDisplayed[obj]; // tooooodo esto es para seguir al mouse correctamente.
    }
    public void OnDragEnd(GameObject obj)
    {
        if (mouseItem.hoverObj)
        {
            inventory.MoveItem(itemsDisplayed[obj], itemsDisplayed[mouseItem.hoverObj]);
        }
        else
        {
            inventory.RemoveItem(itemsDisplayed[obj].item);
        }
        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }
    public void OnDrag(GameObject obj)
    {
        if (mouseItem.obj != null)
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition; //esto es para que el objeto siga al mouse cuando lo agarras.
    }
    //^^ Todo esto de arriba es para que sirvan los botones. Gracias Unity.

    public Vector3 GetPosition(int i)  //esto es para no tener que anotar la posicion local una y otra vez.
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0f);
    }
}

public class MouseItem
{
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
} //para representar al mouse.
