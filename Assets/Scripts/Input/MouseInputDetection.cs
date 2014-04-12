using UnityEngine;
using System.Collections.Generic;

namespace GameInput
{
    /// <summary>
    /// MouseInputDetection предоставялет набор функций для расподнавания пользовательского ввода с помощью мыши
    /// </summary>
    public class MouseInputDetection : InputDetection
    {
        private Vector2? mMousePrevPos;
        private const string LEFT_BUTTON_ID = "-1";

        // Для всех функций принимаюих набор вводимых данных используется первый подходящий по параметрам результат

        public override Movement Detect (IEnumerable<InputEvent> _input)
        {
            if (_input.CountIs (0))
                return new Movement ();

            Drag drag = new Drag();
            float rotate = 0, tilt = 0;

            float zoom = DetectZoom (_input);
            bool hasTap = DetectTap (_input);

            InputEvent inputEvent = _input.WithoutInputType(InputEvent.Type.Scroll).First ();

            if (inputEvent == null)
                return new Movement (drag, zoom, rotate, tilt, hasTap);

            if (mMousePrevPos == null || inputEvent.IsStartEvent)
            {
                if (!inputEvent.IsFinishEvent)
                    mMousePrevPos = inputEvent.CurrentPosition;
                return new Movement (drag, zoom, rotate, tilt, hasTap);
            }

            drag = DetectDrag (inputEvent);
            rotate = DetectRotate (inputEvent);
            tilt = DetectTilt(inputEvent);

            mMousePrevPos = inputEvent.CurrentPosition;

            return new Movement (drag, zoom, rotate, tilt, hasTap);
        }

        protected override void OnReset ()
        {
            mMousePrevPos = null;
        }

        private Drag DetectDrag (InputEvent _inputEvent)
        {
            if (_inputEvent.CurrentType != InputEvent.Type.Drag || 
                _inputEvent.CurrentEventId != LEFT_BUTTON_ID)
                return new Drag ();

            Vector2 scaleFactor = ScaleFactor ();
            Vector2 newPosition = Vector2.Scale (_inputEvent.CurrentPosition, scaleFactor);
            Vector2 prevPosition = Vector2.Scale (mMousePrevPos.Value, scaleFactor);
            return new Drag (newPosition, newPosition - prevPosition);
        }

        private float DetectRotate (InputEvent _inputEvent)
        {
            if (_inputEvent.CurrentType != InputEvent.Type.Drag || 
                _inputEvent.CurrentEventId == LEFT_BUTTON_ID)
                return 0;

            return (_inputEvent.CurrentPosition.x - mMousePrevPos.Value.x) / Screen.width;
        }

        private float DetectTilt (InputEvent _inputEvent)
        {
            if (_inputEvent.CurrentType != InputEvent.Type.Drag || 
                _inputEvent.CurrentEventId == LEFT_BUTTON_ID)
                return 0;

            return (_inputEvent.CurrentPosition.y - mMousePrevPos.Value.y) / Screen.width;
        }

        private float DetectZoom (IEnumerable<InputEvent> _input)
        {
            InputEvent inputEvent = _input.WithInputType (InputEvent.Type.Scroll).First ();
            return inputEvent != null ? inputEvent.ScrollDelta : 0;
        }

        private bool DetectTap (IEnumerable<InputEvent> _input)
        {
            return DetectWithType (_input, InputEvent.Type.Tap);
        }

        private bool DetectWithType (IEnumerable<InputEvent> _input, InputEvent.Type _type)
        {
            return !_input.WithInputType (_type).CountIs(0);
        }
    }
}
