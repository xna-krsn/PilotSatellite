using UnityEngine;
using System.Collections.Generic;

namespace GameInput
{
    public class TouchInputDetection : InputDetection
    {
        // Взаимо исключающие состояния распознования ввода
        private enum Mode 
        {
            No,
            MultiEvent,
            Tilt
        }

        /// <summary>
        /// Погрешности
        /// </summary>
        private const float ANGLE_ERROR = 15;
        private const float DELTA_ERROR = 0.05f;

        private Vector2? mMultiPrevCenter;
        private Vector2? mMultiPrevPos0;
        private Vector2? mMultiPrevPos1;

        private Mode mCurrentMode;
        private int mLastInputCount;

        public override Movement Detect (IEnumerable<InputEvent> _input)
        {
            InputEvent inputEvent0 = null;
            InputEvent inputEvent1 = null;
            Vector2? center = null;

            int inputCount = _input.Count ();

            if (mLastInputCount != inputCount)
                Reset ();

            mLastInputCount = inputCount;

            // вычисляем режим
            switch (inputCount)
            {
            case 0:
                mCurrentMode = Mode.No;
                break;
            case 1:
                if (mCurrentMode == Mode.No || mCurrentMode == Mode.Tilt)
                    mCurrentMode = mMultiPrevCenter == null ? Mode.No : Mode.MultiEvent;
                center = _input.First ().CurrentPosition;
                break;
            case 2:
                _input.GetValues (out inputEvent0, out inputEvent1);
                center = CalcCenter (inputEvent0.CurrentPosition, inputEvent1.CurrentPosition);

                if (mCurrentMode != Mode.No)
                        break;

                if (mMultiPrevPos0 == null || mMultiPrevPos1 == null || mMultiPrevCenter == null ||
                    inputEvent0.CurrentType == InputEvent.Type.Stay || inputEvent1.CurrentType == InputEvent.Type.Stay) 
                {
                    mCurrentMode = Mode.No;
                    break;
                }

                mCurrentMode = CheckForTilt (inputEvent0.CurrentPosition - mMultiPrevPos0.Value, 
                                            inputEvent1.CurrentPosition - mMultiPrevPos1.Value, 
                                            center.Value - mMultiPrevCenter.Value) ? Mode.Tilt : Mode.MultiEvent;
                break;
            default:
                if (mCurrentMode == Mode.No || mCurrentMode == Mode.Tilt)
                    mCurrentMode = Mode.MultiEvent;
                _input.GetValues (out inputEvent0, out inputEvent1);
                center = CalcCenter (inputEvent0.CurrentPosition, inputEvent1.CurrentPosition);
                inputEvent0 = inputEvent1 = null;
                break;
            }

            // вычисления
            Drag drag = new Drag ();
            float zoom = 0, rotate = 0, tilt = 0;
            bool hasTap = false;

            hasTap = !_input.WithInputType (InputEvent.Type.Tap).CountIs (0);

            switch (mCurrentMode)
            {
            case Mode.Tilt:
                if (center == null || mMultiPrevCenter == null)
                    break;
                tilt = CalcTilt (center.Value - mMultiPrevCenter.Value);
                break;
            case Mode.MultiEvent:

                if (mMultiPrevCenter == null)
                    break;

                drag = CalcDrag (mMultiPrevCenter.Value, center.Value);

                if (inputEvent0 == null || inputEvent1 == null || mMultiPrevPos0 == null || mMultiPrevPos1 == null)
                    break;

                Vector2 fromVector = mMultiPrevPos0.Value - mMultiPrevPos1.Value;
                Vector2 toVector = inputEvent0.CurrentPosition - inputEvent1.CurrentPosition;

                zoom = CalcZoom (fromVector, toVector);
                rotate = CalcRotation (fromVector, toVector);

                break;
            }

            // сохраняем текущие значения
            mMultiPrevCenter = center;
            if (inputEvent0 == null || inputEvent1 == null)
            {
                mMultiPrevPos0 = mMultiPrevPos1 = null;
            }
            else
            {
                mMultiPrevPos0 = inputEvent0.CurrentPosition;
                mMultiPrevPos1 = inputEvent1.CurrentPosition;
            }

            return new Movement(drag, zoom, rotate, tilt, hasTap);
        }

        protected override void OnReset ()
        {
            mMultiPrevPos0 = mMultiPrevPos1 = mMultiPrevCenter = null;
            mCurrentMode = Mode.No;
            mLastInputCount = 0;
        }


        private Vector2 CalcCenter (Vector2 _point0, Vector2 _point1)
        {
            return _point0 + (_point1 - _point0) / 2; 
        }

        private Drag CalcDrag (Vector2 _from, Vector2 _to)
        {
            Vector2 scaleFactor = ScaleFactor ();
            Vector2 newPosition = Vector2.Scale (_to, scaleFactor);
            Vector2 prevPosotion = Vector2.Scale (_from, scaleFactor);
            return new Drag (newPosition, newPosition - prevPosotion);
        }

        private float CalcZoom (Vector2 _from, Vector2 _to)
        {
            // значение увеличения это изменение растояния между пальцами
            Vector2 scaleFactor = ScaleFactor();
            float prevDiff = Vector2.Scale (_from, scaleFactor).magnitude;
            float diff = Vector2.Scale(_to, scaleFactor).magnitude;
            return (diff - prevDiff);
        }

        private float CalcRotation (Vector2 _from, Vector2 _to)
        {
            // значение поворота это угол изменения положения пальцев друг относительно друга
            float angle;
            Vector3 axis;
            Quaternion.FromToRotation (_from, _to).ToAngleAxis(out angle, out axis);
            if (axis.z < 0)
                angle *= -1;
            return angle / 180;
        }

        private bool CheckForTilt(Vector2 _direction0, Vector2 _direction1, Vector2 _deltaCenter)
        {
            // вычисляем угол отклонения от вертикали
            float yAngle = Vector2.Angle(_deltaCenter, Vector2.up);

            // проверка на коллинеарность направления движения для пальцев
            bool isColinear = Mathf.Abs(Vector3.Angle(_direction0, _direction1)) < ANGLE_ERROR;  
            // проверка на вертикальность направления пальцев
            bool isVertical = yAngle < ANGLE_ERROR || Mathf.Abs(yAngle - 180) < ANGLE_ERROR;

            // в случае если направеления пальцев коллинеарны и вертикальны - 
            // мы можем утверждать, что пользователь хочет использовать Tilt
            return isColinear && isVertical; 
        }

        private float CalcTilt(Vector2 _detaCenter)
        {
            // наклон - это изменение усредненного значения ввода для 2ух точек по вертикали
            return _detaCenter.y / Screen.height;
        }
    }
}

