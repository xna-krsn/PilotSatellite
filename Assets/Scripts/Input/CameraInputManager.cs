using System.Collections.Generic;

namespace GameInput
{
    public class CameraInputManager : IInputListener
    {
        /// <summary>
        /// Тип выбранного CameraMover
        /// </summary>
        public string CurrentMoverType { get; private set; }

        /// <summary>
        /// Выбранный CameraMover
        /// </summary>
        public CameraMover CurrentMover 
        {
            get 
            {
                CameraMover mover;
                return CurrentMoverType != null && mMoverCache.TryGetValue (CurrentMoverType, out mover) ? mover : null; 
            }
        }

        private Dictionary<string, CameraMover> mMoverCache = 
            new Dictionary<string, CameraMover>();


        public CameraInputManager (params CameraMover[] _movers)
        {
            Enabled = true;
            // кэшируем 
            for (int i = 0; i < _movers.Length; i++)
            {
                mMoverCache.Add (_movers[i].Type, _movers[i]);
                _movers[i].Enable (false);
            }
        }

        /// <summary>
        /// Переключает Mover по типу, сообщает об успешности операции
        /// </summary>
        public bool SwitchMover (string _type)
        {
            if (CurrentMoverType == _type)
                return true;

            CameraMover mover;
            if (!mMoverCache.TryGetValue (_type, out mover))
                return false;

            mover.GetControll (CurrentMover);
            CurrentMoverType = _type;
            return true;
        }
            
        #region IInputListener

        private const int PRIORITY = -10;

        public int Priority { get { return PRIORITY; } }

        public bool Enabled { get; set; }

        public bool OnInputCatched (IEnumerable<InputEvent> _input)
        {
            if (!Enabled)
                return false;

            return CurrentMover.OnGetInput (_input);
        }

        public void OnInputFinished ()
        {
            Reset ();
        }

        public void Reset ()
        {
            CurrentMover.Reset ();
        }

        #endregion
    }
}