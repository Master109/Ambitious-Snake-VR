using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class ColorExtensions
	{
		public static Color GetWithAlpha (this Color c, float a)
		{
			return new Color(c.r, c.g, c.b, a);
		}
		
		public static Color GetWithAddedAlpha (this Color c, float a)
		{
			return c.GetWithAlpha(c.a + a);
		}
		
		public static Color GetWithMultipliedAlpha (this Color c, float a)
		{
			return c.GetWithAlpha(c.a * a);
		}
		
		public static Color GetWithDividedAlpha (this Color c, float a)
		{
			return c.GetWithAlpha(c.a / a);
		}

		public static Color Add (this Color c, float f)
		{
			Color output = c;
			output.r += f;
			output.g += f;
			output.b += f;
			return output;
		}
		
		public static Color Multiply (this Color c, float f)
		{
			Color output = c;
			output.r *= f;
			output.g *= f;
			output.b *= f;
			return output;
		}
		
		public static Color Divide (this Color c, float f)
		{
			Color output = c;
			output.r /= f;
			output.g /= f;
			output.b /= f;
			return output;
		}
		
		public static byte[] ToBytes (this Color color)
		{
			byte[] bytes = new byte[4];
			bytes[0] = (byte) (255 * color.r - 128);
			return bytes;
		}

		public static Color RandomColor ()
		{
			return new Color(Random.value, Random.value, Random.value);
		}
	}
}