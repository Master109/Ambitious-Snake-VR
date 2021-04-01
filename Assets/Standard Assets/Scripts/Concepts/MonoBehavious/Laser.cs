using UnityEngine;

namespace AmbitiousSnake
{
	public class Laser : UpdateWhileEnabled
	{
		public LineRenderer line;
		public LayerMask whatBlocksMe;
		public Transform trs;
		public RaycastHit hit;
		const int MAX_LENGTH = int.MaxValue;

		public override void DoUpdate ()
		{
			if (Physics.Raycast(trs.position, trs.forward, out hit, MAX_LENGTH, whatBlocksMe))
				line.SetPosition(1, hit.point);
			else
				line.SetPosition(1, trs.position + trs.forward * MAX_LENGTH);
		}
	}
}