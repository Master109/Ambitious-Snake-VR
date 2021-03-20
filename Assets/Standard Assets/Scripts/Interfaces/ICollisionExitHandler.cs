using UnityEngine;

namespace AmbitiousSnake
{
	public interface ICollisionExitHandler
	{
        Collider Collider { get; }
        
        void OnCollisionExit (Collision coll);
	}
}