using System.Collections.Generic;
using UnityEngine;

namespace GameInput 
{
    /// <summary>
    /// Абстракция для адаптера, предоставляющего события для InputManager
    /// Годный адаптер должен предоставлять события таким обрахзом, 
    /// чтобы единовременно не могло сущестовать нескольких событий с одним Id.
    /// </summary>
    public abstract class InputAdapeter : MonoBehaviour
    {
        // востанавливаем отсутствующие эвенты
        private SortedDictionary <string, InputEvent> mEvents = 
            new SortedDictionary<string, InputEvent> ();

        public abstract int InputCount { get; }

        public bool HasInput { get; private set; }
        
        private InputManager mInputManager;
        
        /// <summary>
        /// Единственно верный способ создать адаптер ввода
        /// </summary>
        public static T Create<T> (InputManager _inputManager, GameObject _atachTo = null) where T : InputAdapeter
        {
            GameObject go = _atachTo;
            if (go == null) 
                go = new GameObject("InputAdapter");
            T inputAdapter = go.AddComponent<T>();
            inputAdapter.SetInputManager(_inputManager);
            return inputAdapter;
        }
        
        protected void SetInputManager (InputManager _inputManager)
        {
            mInputManager = _inputManager;
        }

        void Update ()
        {
            if (mInputManager == null)
                throw new System.Exception ("Не проинициализированный объект. Для создания объекта воспользуйтесь InputAdapter.Create");

            // нам нужны только кадры в которых произошло хоть одно событие
            if (!HasInput)
            {
                if (InputCount == 0)
                    mEvents.Clear ();
                return;
            }

            HasInput = false;

            mInputManager.FireEvents (new LinkedList<InputEvent>(mEvents.Values));

            // обновляем список событий 
            LinkedList<string> removeList = new LinkedList<string> ();
            foreach (string id in mEvents.Keys) 
            {
                InputEvent inputEvent = mEvents [id];

                // неактуьные помечаем для удаления
                if (inputEvent.IsFinishEvent)
                    removeList.AddLast (inputEvent.CurrentEventId);
                // старые опмечаем как "Stay"
                else if (inputEvent.CurrentType != InputEvent.Type.Stay)
                    mEvents [inputEvent.CurrentEventId] = CreateStayEvent(inputEvent);
            }

            // очищаем завершенные действия
            foreach (string removeId in removeList)
                mEvents.Remove (removeId);
        }

        /// <summary>
        /// Добавляет эвент непосредственно в InputManager
        /// </summary>
        protected void AddEvent (InputEvent _event)
        {
            HasInput = true;
            mEvents [_event.CurrentEventId] = _event;
        }

        private InputEvent CreateStayEvent (InputEvent _inputEvent)
        {
            return new InputEvent (InputEvent.Type.Stay, _inputEvent.CurrentEventId, _inputEvent.CurrentPosition);
        }
    }
}