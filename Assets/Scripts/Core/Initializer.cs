using UnityEngine;
using System.Collections;
using GameInput; 

public class Initializer : MonoBehaviour 
{
    InputManager mInputManager; 
    ViewInput mViewInput;
    UnityInputAdapter mInputAdapter;
    
    void Start ()
    {
        mInputManager = new InputManager();
        mViewInput = new ViewInput(mInputManager); 
        mInputAdapter = InputAdapeter.Create<UnityInputAdapter>(mInputManager);
    }
}
