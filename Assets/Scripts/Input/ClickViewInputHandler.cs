using UnityEngine;
using System.Collections;
using GameInput; 

public class ClickViewInputHandler : ViewInputHandler
{ 
    public const string TYPE = "click_view";

    public override string Type 
    {
        get { return TYPE; }
    }
    
    internal override bool Handle (ViewInputData _data)
    {
        Debug.Log (_data);
        if (_data.Event.CurrentType == InputEvent.Type.Tap && _data.CollisionObject != null) 
        {
            _data.CollisionObject.Click ();
            return true;
        }
        
        return false;
    }
    
    internal override void HandleInputFinished () {}
    
    internal override void Reset () {}
}
