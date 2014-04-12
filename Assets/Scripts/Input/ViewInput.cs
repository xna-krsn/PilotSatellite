using UnityEngine;
using System.Collections;
using GameInput; 

public class ViewInput 
{
    public ViewInputManager Manager { get; private set; }
    
    public ViewInput (InputManager _inputManager)
    {
        Manager = new ViewInputManager(new ClickViewInputHandler());
        Manager.SwitchHandler (ClickViewInputHandler.TYPE);
        _inputManager.AddListener (Manager);
    }
}
