using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using AmbitiousSnake;

public class LookAtVRCameraRig : UpdateWhileEnabled, IUpdatable
{
	public Transform trs;
	
	public override void DoUpdate ()
	{
		trs.forward = VRCameraRig.Instance.eyesTrs.position - trs.position;
	}
}
