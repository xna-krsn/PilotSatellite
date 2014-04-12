using UnityEngine;
using System.Collections.Generic;
using GameInput;

/// <summary>
/// Адаптер ввода для Unity
/// </summary>
public class UnityInputAdapter : InputAdapeter
{
    // здесь хранится типы событий ввода по ID до их завершения
    private Dictionary<KeyCode, InputEvent.Type> mInputByEventType = 
        new Dictionary<KeyCode, InputEvent.Type>();
    
    public override int InputCount 
    {
        get { return 1; }
    }
    
    void OnGUI()
    {
        Event currentEvent = Event.current;
        
        // обраотка событий только для мышки
        if (currentEvent.isMouse)
        {
            switch (currentEvent.type)
            {
            case EventType.MouseDown:
                AddEvent (CreateEvent(InputEvent.Type.Press, currentEvent.keyCode));
                RegisterEvent (currentEvent, InputEvent.Type.Press);
                break;
            case EventType.MouseDrag:
                AddEvent (CreateEvent(InputEvent.Type.Drag, currentEvent.keyCode));
                RegisterEvent (currentEvent, InputEvent.Type.Drag);
                break;
            case EventType.MouseUp:
                AddEvent (CreateEvent(GetFinisEvent(currentEvent), currentEvent.keyCode));
                UnregisterEvent (currentEvent);
                break;
            case EventType.ScrollWheel:
                AddEvent (CreateEvent(InputEvent.Type.Scroll, currentEvent.keyCode));
                break;
            }
        }
    }
    
    #region Определяем тип завершающего события
    
    private void RegisterEvent (Event _event, InputEvent.Type _type)
    {
        mInputByEventType[_event.keyCode] = _type;
    }
    
    private InputEvent.Type GetFinisEvent (Event _event)
    { 
        InputEvent.Type result;
        if (!mInputByEventType.TryGetValue (_event.keyCode, out result))
            return InputEvent.Type.Release; 
        if (result == InputEvent.Type.Press)
            return InputEvent.Type.Tap; 
        return InputEvent.Type.Release; 
    }
    
    private void UnregisterEvent (Event _event)
    {
        mInputByEventType.Remove (_event.keyCode);
    }
    
    #endregion
    
    private InputEvent CreateEvent (InputEvent.Type _type, KeyCode _id, float? _deltaScroll = null)
    {
        return new InputEvent (_type, _id.ToString(), Input.mousePosition, _deltaScroll ?? 0);
    }
}
