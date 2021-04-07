using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class MathfExtensions
	{
		public const int NULL_INT = 1234567890;
		public const float NULL_FLOAT = NULL_INT;
		public const float INCHES_TO_CENTIMETERS = 2.54f;
		
		public static float SnapToInterval (float f, float interval)
		{
			if (interval == 0)
				return f;
			else
				return Mathf.Round(f / interval) * interval;
		}
		
		public static int Sign (float f)
		{
			if (f == 0)
				return 0;
			else
				return (int) Mathf.Sign(f);
		}
		
		public static bool AreOppositeSigns (float f1, float f2)
		{
			return Mathf.Abs(Sign(f1) - Sign(f2)) == 2;
		}

		public static float GetClosestNumber (float f, params float[] numbers)
		{
			float closestNumber = numbers[0];
			float closestDistance = Mathf.Abs(f - closestNumber);
			for (int i = 1; i < numbers.Length; i ++)
			{
				float number = numbers[i];
				float distance = Mathf.Abs(f - number);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestNumber = number;
				}
			}
			return closestNumber;
		}

		public static int GetIndexOfClosestNumber (float f, params float[] numbers)
		{
			int output = 0;
			float closestNumber = numbers[0];
			float closestDistance = Mathf.Abs(f - closestNumber);
			for (int i = 1; i < numbers.Length; i ++)
			{
				float number = numbers[i];
				float distance = Mathf.Abs(f - number);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestNumber = number;
					output = i;
				}
			}
			return output;
		}

		public static float RegularizeAngle (float angle)
		{
			while (angle >= 360 || angle < 0)
				angle += Mathf.Sign(360 - angle) * 360;
			return angle;
		}

		public static float ClampAngle (float ang, float min, float max)
		{
			ang = RegularizeAngle(ang);
			min = RegularizeAngle(min);
			max = RegularizeAngle(max);
			float minDist = Mathf.Min(Mathf.DeltaAngle(ang, min), Mathf.DeltaAngle(ang, max));
			float _ang = WrapAngle(ang + Mathf.DeltaAngle(ang, minDist));
			if (_ang == min)
				return min;
			else if (_ang == max)
				return max;
			else
				return ang;
		}

		public static float WrapAngle (float ang)
		{
			if (ang < 0)
				ang += 360;
			else if (ang > 360)
				ang = 360 - ang;
			return ang;
		}

		public static float Round (float f, RoundingMethod roundingMethod)
		{
			if (roundingMethod == RoundingMethod.HalfOrMoreRoundsUp)
			{
				if (f % 1 >= 0.5f)
					return Mathf.Round(f);
			}
			else if (roundingMethod == RoundingMethod.HalfOrLessRoundsDown)
			{
				if (f % 1 <= 0.5f)
					return (int) f;
			}
			else if (roundingMethod == RoundingMethod.RoundUpIfNotInteger)
			{
				if (f % 1 != 0)
					return Mathf.Round(f);
			}
			else// if (roundingMethod == RoundingMethod.RoundDownIfNotInteger)
			{
				if (f % 1 != 0)
					return (int) f;
			}
			return f;
		}
		
		public enum RoundingMethod
		{
			HalfOrMoreRoundsUp,
			HalfOrLessRoundsDown,
			RoundUpIfNotInteger,
			RoundDownIfNotInteger
		}
	}
}