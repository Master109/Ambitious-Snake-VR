using UnityEngine;

namespace AmbitiousSnake
{
	public interface ICollisionEnterHandler
	{
        Collider Collider { get; }
        
        void OnCollisionEnter (Collision coll);
	}
}