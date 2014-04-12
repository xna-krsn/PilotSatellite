using UnityEngine;
using System.Collections.Generic;

namespace GameInput
{
    /// <summary>
    /// Предоставляет данные для работы
    /// </summary>
    public class ViewInputData
    {
        public InputEvent Event { get; private set; }
        public Clickable CollisionObject { get; private set; }

        public ViewInputData (InputEvent _event, Clickable _clicable)
        {
            Event = _event;
            CollisionObject = _clicable;
        }

        public override string ToString ()
        {
            return string.Format ("[InputData: Event={0}, CollisionObject={1}]", 
                Event, CollisionObject);
        }
    }

    public abstract class ViewInputHandler
    {
        /// <summary>
        /// ID режим работы обработчика
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// Обработать ввода
        /// </summary>
        internal abstract bool Handle (ViewInputData _data);

        /// <summary>
        /// Обработка завершения ввода
        /// </summary>
        internal virtual void HandleInputFinished () {}

        /// <summary>
        /// Возвращает объект в изначальное состояние
        /// </summary>
        internal abstract void Reset ();
    }

    /// <summary>
    /// Предоставляет ввод для объектов карты
    /// </summary>
    public class ViewInputManager : IInputListener
    { 
        public const int MAX_DISTANCE = 1000;
        
        private Dictionary<string, ViewInputHandler> mHandlers = 
            new Dictionary<string, ViewInputHandler>();

        /// <summary>
        /// ID выбранного обработчика
        /// </summary>
        public string CurrentHandlerType { get; private set; }

        /// <summary>
        /// Выбранный обработчик
        /// </summary>
        public ViewInputHandler CurrentHandler 
        {
            get 
            {
                ViewInputHandler handler;
                return mHandlers.TryGetValue (CurrentHandlerType, out handler) ? handler : null;
            }
        }

        public bool Enabled { get; set; } 
    	
        public int Priority  { get { return 0; } }

        public ViewInputManager(params ViewInputHandler[] _handlers)
    	{
            Enabled = true;

            for (int i = 0; i < _handlers.Length; i++)
                mHandlers [_handlers [i].Type] = _handlers [i];
    	}

        /// <summary>
        /// Усанавливает режим для отображения 
        /// </summary>
        public void SwitchHandler (string _mode)
        {
            CurrentHandlerType = _mode;
        }

        public bool OnInputCatched (IEnumerable<InputEvent> _input)
        {
            if (!Enabled)
                return false;

            if (!_input.CountIs (1)) 
            {
                Reset ();
                return false;
            }

            InputEvent inputEvent = _input.First ();
            Clickable clickable = GetByRaycast (inputEvent.CurrentPosition, Clickable.LAYER_NAME); 
            ViewInputData inputData = new ViewInputData (inputEvent, clickable);

            return CurrentHandler.Handle (inputData);
    	}

        public void OnInputFinished () 
        {
            CurrentHandler.HandleInputFinished ();
        }

        public void Reset()
        {
            CurrentHandler.Reset ();
        }
        
        private Clickable GetByRaycast (Vector3 _screenPoint, string _layerName)
        {
            Ray ray = Camera.main.ScreenPointToRay (_screenPoint); 
            RaycastHit hit;
            if (!Physics.Raycast (ray, out hit, MAX_DISTANCE, 1 << LayerMask.NameToLayer(_layerName)))
                return null;
            return hit.collider.GetComponent<Clickable>();
        }
    }
}