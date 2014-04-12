using UnityEngine;
using System.Collections.Generic;

namespace GameInput
{
    /// <summary>
    /// Клас предоставляет обработчик ввода для кмеры
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public abstract class CameraMover : MonoBehaviour
    {
        [System.Flags]
        public enum InputFilter
        {
            Nothing = 0,
            Move = 1,
            Zoom = 2,
            Rotate = 4,
            Tilt = 8,
            Tap = 16,
            DoubleTap = 32,
            All = ~0
        }

        public InputFilter Filters { get; set; }

        private const float MAX_LAG_TIMR_VALUE = 0.1f;

        /// <summary>
        /// ID режима работы CameraMover
        /// </summary>
        public abstract string Type { get; }

        void Awake()
        {
            Filters = InputFilter.All;
            OnAwake ();
        }

        void Update ()
        {
            // когда лагает игнорируем ввод
            if (Time.deltaTime > MAX_LAG_TIMR_VALUE) 
                return;

            OnUpdate ();
        }

        internal void GetControll (CameraMover _fromMover) 
        {
            if (_fromMover != null)
                Enable (false);

            Enable (true);
        }

        /// <summary>
        /// Включает/отключает поведение объекта
        /// </summary>
        public void Enable (bool _value)
        {
            enabled = _value;
            Reset ();
        }
            
        /// <summary>
        /// Очищает все внутренние буферы
        /// </summary>
        protected internal void Reset () 
        {
            OnReset ();
        }

        internal bool OnGetInput (IEnumerable<InputEvent> _events)
        {
            if (_events == null)
                throw new System.ArgumentNullException ();
            if (_events.CountIs (0))
                return false;

            return ProcessInput(_events);
        }

        // TODO: вынести в утилитарный класс
        protected float GetDeltaFromInterval (float _from, float _to, float _value)
        {
            float result = Mathf.Lerp (_from, _to, Mathf.Abs(_value)) - _from;
            if (_value < 0)
                result *= -1;
            return result;
        }

        /// <summary>
        /// Обработчик ввода
        /// </summary>
        protected abstract bool ProcessInput (IEnumerable<InputEvent> _input);

        protected abstract void OnReset ();

        protected virtual void OnUpdate () {}

        protected virtual void OnAwake() {}
    }
}