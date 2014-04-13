using System;
using UnityEngine;

namespace Assets.Scripts.Planets
{
	public class Planet
	{
        private const float GravitationalConstant = 6.674e-11f;
        private const float SunMass = 1.989e+30f;

	    public GameObject Planet3D;
        public readonly string Name;
        public readonly float Mass;
	    public readonly float MeanRadius;
	    public readonly float SunDistance; 

	    public Vector3 CurrentPosition;
	    public Vector3 CurrentVelocity;
	    private readonly Vector3 _acceleration;

	    public Planet(string name, float mass, float radius, float distance)
	    {
	        Name = name;
	        Mass = mass;
	        MeanRadius = radius;
	        SunDistance = distance;
            _acceleration = GravitationalConstant * SunMass / SunDistance * Vector3.one;
            Debug.Log("Mean Distance = " + distance);
            //Debug.Log("Current Distance = " + position.magnitude);
	    }

	    public Vector3 GetCurrentPosition(float deltaTime)
	    {
	        CurrentVelocity += _acceleration * deltaTime;
            CurrentPosition += CurrentVelocity * deltaTime;
            return CurrentPosition;
	    }
	}

    public class BigNumber
    {
        public float Number { get; set; }
        public float Power { get; set; }

        public static BigNumber operator *(BigNumber number1, BigNumber number2)
        {
            return new BigNumber {Number = number1.Number * number2.Number, Power = number1.Power + number2.Power};
        }

        public static BigNumber operator /(BigNumber number1, BigNumber number2)
        {
            return new BigNumber { Number = number1.Number / number2.Number, Power = number1.Power - number2.Power };
        }

        public static implicit operator float(BigNumber number)
        {
            return number.Number * (float)Math.Pow(10, number.Power);
        }
    }
}
