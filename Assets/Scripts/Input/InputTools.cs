using System.Collections.Generic;
using System.Linq;

namespace GameInput
{
    public static class InputTools
    { 
        #region Запросы к колеции объектов событий ввода

        public static bool IsInputFinished (this IEnumerable<InputEvent> _source)
        {
            if (_source == null)
                throw new System.NullReferenceException ();

            foreach (InputEvent inputEvent in _source)
                if (!inputEvent.IsFinishEvent)
                    return false;

            return true;
        }

        public static IEnumerable<InputEvent> WithInputType (this IEnumerable<InputEvent> _source, params InputEvent.Type[] _types)
        {
            if (_source == null)
                throw new System.NullReferenceException ();

            LinkedList<InputEvent> result = new LinkedList<InputEvent> ();

            foreach (InputEvent inputEvent in _source)
                if (System.Array.IndexOf (_types, inputEvent.CurrentType) >= 0)
                    result.AddLast(inputEvent);

            return result;
        }

        public static IEnumerable<InputEvent> WithoutInputType (this IEnumerable<InputEvent> _source, params InputEvent.Type[] _types)
        {
            if (_source == null)
                throw new System.NullReferenceException ();

            LinkedList<InputEvent> result = new LinkedList<InputEvent> ();

            foreach (InputEvent inputEvent in _source)
                if (System.Array.IndexOf (_types, inputEvent.CurrentType) < 0)
                    result.AddLast(inputEvent);

            return result;
        }

        public static IEnumerable<InputEvent> WithInputId (this IEnumerable<InputEvent> _source, string _id)
        {
            if (_source == null)
                throw new System.NullReferenceException ();

            LinkedList<InputEvent> result = new LinkedList<InputEvent> ();

            foreach (InputEvent inputEvent in _source)
                if (inputEvent.CurrentEventId == _id)
                    result.AddLast(inputEvent);

            return result;
        }
            
        public static IEnumerable<InputEvent> WithoutInputId (this IEnumerable<InputEvent> _source, string _id)
        {
            if (_source == null)
                throw new System.NullReferenceException ();

            LinkedList<InputEvent> result = new LinkedList<InputEvent> ();

            foreach (InputEvent inputEvent in _source)
                if (inputEvent.CurrentEventId != _id)
                    result.AddLast(inputEvent);

            return result;
        }

        #endregion
         
        /// <summary>
        /// Перегружающая обертка над GetValues<T>, позволяющая получать первое значение
        /// </summary>
        public static T First<T> (this IEnumerable<T> _source)
        {
            T[] param = new T[1];
            GetValues (_source, ref param);
            return param[0];
        }

        /// <summary>
        /// Перегружающая обертка над GetValues<T>, позволяющая получать 2 первых значения
        /// </summary>
        public static bool GetValues<T> (this IEnumerable<T> _source, out T _first, out T _second)
        {
            _first = _second = default(T);

            T[] param = new T[2];
            if (!GetValues (_source, ref param))
                return false;
            _first = param [0];
            _second = param [1];
            return true;
        }

        /// <summary>
        /// Позволяет безопасно получить параметры в виде массива
        /// </summary>
        public static bool GetValues<T> (this IEnumerable<T> _source, ref T[] _values)
        {
            if (_source == null)
                throw new System.NullReferenceException ();
            int index = 0;
            foreach (T data in _source)
            {
                if (_values.Length == index)
                    break;
                _values [index++] = data;
            }

            return index == _values.Length;
        }

        /// <summary>
        /// Проверяет является размер колекции раыным ожидаемому.
        /// Оптимизация для функции вычисления размера позволяющая избежать лишних итераций
        /// </summary>
        public static bool CountIs<T> (this IEnumerable<T> _source, int _count)
        {
            if (_source == null)
                throw new System.NullReferenceException ();

            int counter = 0;
            IEnumerator<T> enumirator = _source.GetEnumerator();
            while (enumirator.MoveNext())
                if (++counter > _count)
                    break;

            return counter == _count;
        }

        /// <summary>
        /// Вычисление размера коллекции
        /// </summary>
        public static int Count<T> (this IEnumerable<T> _source)
        {
            if (_source == null)
                throw new System.NullReferenceException ();

            int counter = 0;
            IEnumerator<T> enumirator = _source.GetEnumerator();
            while (enumirator.MoveNext())
                counter++;
            return counter;
        }
    }
}

