using UnityEngine;

namespace AmbitiousSnake
{
	public class GravityAndRotationTutorial : Tutorial
	{
		void OnTriggerEnter (Collider other)
		{
			Finish ();
		}
	}
}