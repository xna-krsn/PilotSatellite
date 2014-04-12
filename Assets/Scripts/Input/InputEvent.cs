using UnityEngine;

namespace GameInput
{
    public class InputEvent 
    {
        public enum Type
        {
            Drag,
            Scroll,
            Tap,
            Press,
            Release,
            Stay
        }

        public Type CurrentType { get; private set; }
        public string CurrentEventId { get; private set; }
        public Vector2 CurrentPosition { get; private set; }
        public float ScrollDelta { get; private set; }

        /// <summary>
        /// Определяет является ли событие стартовым для ввода
        /// </summary>
        public bool IsStartEvent
        {
            get { return CurrentType == Type.Press || CurrentType == Type.Scroll; }
        }

        /// <summary>
        /// Определяет является ли событие завершаюим для ввода 
        /// </summary>
        public bool IsFinishEvent 
        {
            get { return CurrentType == Type.Release || CurrentType == Type.Tap || CurrentType == Type.Scroll; }
        }

        public InputEvent (Type _type, string _id, Vector2 _position, float _scrollDelta = 0)
        {
            CurrentType = _type;
            CurrentEventId = _id;
            CurrentPosition = _position;
            ScrollDelta = _scrollDelta;
        }

        public override string ToString()
        {
            return string.Format("[InputEvent: CurrentType={0}, CurrentEventId={1}, CurrentPosition={2}, ScrollDelta={3}]", 
                CurrentType, CurrentEventId, CurrentPosition, ScrollDelta);
        }
    }
}