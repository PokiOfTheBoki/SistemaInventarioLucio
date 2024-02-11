using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver //Con esta Script vamos a convertir al Empty en un Item que yace en el suelo.
{
    public ItemObject item;

public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = item.uiDisplay;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
    }
}
