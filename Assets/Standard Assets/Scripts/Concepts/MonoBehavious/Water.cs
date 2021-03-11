using UnityEngine;

namespace AmbitiousSnake
{
	public class Water : MonoBehaviour
	{
		public virtual void OnTriggerEnter (Collider other)
		{
			CharacterController controller = other.GetComponentInParent<CharacterController>();
			if (controller != null)
			// {
				controller.enabled = false;
			// 	Player Player = other.GetComponent<Player>();
			// 	if (Player != null)
			// 		Player.isSwimming = true;
			// }
		}
		
		public virtual void OnTriggerExit (Collider other)
		{
			CharacterController controller = other.GetComponentInParent<CharacterController>();
			if (controller != null)
			// {
				controller.enabled = true;
			// 	Player Player = other.GetComponent<Player>();
			// 	if (Player != null)
			// 		Player.isSwimming = false;
			// }
		}
	}
}