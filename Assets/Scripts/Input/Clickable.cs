using UnityEngine;
using System.Collections;

public class Clickable : MonoBehaviour 
{
    public const string LAYER_NAME = "Clickable";
    
    public event System.Action<Clickable> Clicked;
 
    void Awake ()
    {
        gameObject.layer = LayerMask.NameToLayer (LAYER_NAME);
    }
    
    public void Click ()
    {
        if (Clicked != null)
            Clicked (this);
    }
}