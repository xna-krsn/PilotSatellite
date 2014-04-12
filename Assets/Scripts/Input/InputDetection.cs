using UnityEngine;
using System.Collections.Generic;

namespace GameInput
{
    public static class InputDetectionFactory 
    {
        // маленькая фабрика
        public static InputDetection GetInputDetection()
        {
//            if (Application.isEditor || Application.isWebPlayer)
//                return new MouseInputDetection ();

            return new TouchInputDetection ();
        }
    }

    /// <summary>
    /// Базовый абстрактный класс, предоставляющий интерфейс распознования 
    /// и определяющий общий шаблон его поведения
    /// </summary>
    public abstract class InputDetection
    {
        /// <summary>
        /// Структура для представления движения 'Drag'
        /// </summary>
        public struct Drag
        {
            public Vector2 Delta { get; private set; }
            public Vector2 Position { get; private set; } 

            public Drag (Vector2 _position, Vector2 _delta) : this ()
            {
                Position = _position;
                Delta = _delta;
            }

            public override string ToString ()
            {
                return string.Format ("[Drag: Delta={0}, Position={1}]", Delta, Position);
            }
        }

        /// <summary>
        /// Структура для пердоставления результата распознавания движения
        /// </summary>
        public struct Movement
        {
            public Drag DragValue { get; private set; }
            public float ZoomValue { get; private set; }
            public float RotateValue { get; private set; }
            public float TiltValue { get; private set; }
            public bool HasTap { get; private set; }

            public Movement (Drag _drag, float _zoom, float _rotate, float _tilt, bool _hasTap) : this ()
            {
                DragValue = _drag;
                ZoomValue = _zoom;
                RotateValue = _rotate;
                TiltValue = _tilt;
                HasTap = _hasTap;
            }

            public override string ToString ()
            {
                return string.Format ("[Detection: DragValue={0}, ZoomValue={1}, RotateValue={2}, TiltValue={3}]", 
                    DragValue, ZoomValue, RotateValue, TiltValue);
            }
        }

        /// <summary>
        /// Очищает всю уэшируемые данные
        /// </summary>
        public void Reset ()
        {
            OnReset ();
        }

        /// <summary>
        /// Распознает ввод
        /// </summary>
        public abstract Movement Detect (IEnumerable<InputEvent> _input);

        protected abstract void OnReset ();

        protected Vector2 ScaleFactor()
        {
            return new Vector2 (1 / (float)Screen.width, 1 / (float)Screen.height);
        }
    }
}