using System.Collections.Generic;

namespace GameInput
{
    /// <summary>
    /// Общий интрефейс для обработчиков событий ввода
    /// </summary>
	public interface IInputListener
	{	
		/// <summary>
        /// Приоритет для обработки ввода
		/// </summary>
		int Priority { get; }

		/// <summary>
		/// Обрабатывает ли объект ввод
		/// </summary>
	    bool Enabled { get; set; }

		/// <summary>
        /// Обработчик ввода
		/// </summary>
        bool OnInputCatched (IEnumerable<InputEvent> _input);

        /// <summary>
        /// Сообщает об окончание ввода
        /// </summary>
        void OnInputFinished ();

        /// <summary>
        /// Возвращает объект в исходное состояния 
        /// </summary>
        void Reset ();
	}

    public class InputManager
	{ 
		/// <summary>
		/// Sorted by priority list of Input Listeners
		/// </summary>
		private LinkedList<IInputListener> mListenerList = 
			new LinkedList<IInputListener>();

        private IInputListener mPrimeListener;

        /// <summary>
        /// Запускает все эвенты, после чего очищает список эвентов
        /// </summary>
        internal void FireEvents (IEnumerable<InputEvent> _input)
	    {
            if (_input.CountIs(0)) 
                return;

            NotifyListeners (_input);

            if (_input.IsInputFinished ())
                InputFinished ();
        }

		/// <summary>
		/// Adds the listener to sorted by priority list
		/// </summary>
		public void AddListener(IInputListener _listener)
		{
	        if (_listener == null)
	            throw new System.ArgumentNullException ("_listener");
			
			LinkedListNode<IInputListener> lessNode = mListenerList.First;
			
			while (lessNode != null) 
			{
                if (lessNode.Value == _listener)
                    return;
				if (lessNode.Value.Priority < _listener.Priority)
					break;

				lessNode = lessNode.Next;
			}
			LinkedListNode<IInputListener> newNode = new LinkedListNode<IInputListener>(_listener);
			
			if (lessNode == null)
				mListenerList.AddLast(newNode);
			else
				mListenerList.AddBefore(lessNode, newNode);
		}
		
		/// <summary>
		/// Removes the listener.
		/// </summary>
		public void RemoveListener(IInputListener _listener)
		{
            mListenerList.Remove(_listener);
		}

        /// <summary>
        /// Clears the listeners.
        /// </summary>
        public void ClearListeners()
        {
            mListenerList.Clear ();
        }

        private void ResetPrimeListener ()
        { 
            if (mPrimeListener != null)
                mPrimeListener.Reset ();
            mPrimeListener = null;
        }

        private void InputFinished ()
        {
            foreach (IInputListener inputListener in mListenerList)
                inputListener.OnInputFinished ();
            mPrimeListener = null;
        }
		
        private void NotifyListeners (IEnumerable<InputEvent> _input)
        {
            // пытаемся передать ввод захватившему его слушателю
            if (mPrimeListener != null && mPrimeListener.OnInputCatched (_input))
                return;

            // пытаемся определить слушателя
            IInputListener nextPrimeListener = null;
            foreach (IInputListener listener in mListenerList)
            {
                if (!listener.Enabled || mPrimeListener == listener) 
                    continue;

                if (listener.OnInputCatched (_input)) 
                {
                    nextPrimeListener = listener;
                    break;
                }
            }

            // возвращаем прошлого основного слушателя в исходное состояния
            ResetPrimeListener ();

            // сохраняем новго слушателя
            mPrimeListener = nextPrimeListener;
		}
	}
}