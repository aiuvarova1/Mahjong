using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SliderDrag : MonoBehaviour, IPointerUpHandler
{
    //indicates end of slider moving
    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerPrefs.instance.WriteFileData();
    }
}
