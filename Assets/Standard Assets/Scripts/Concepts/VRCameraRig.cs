using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEngine.InputSystem;

namespace AmbitiousSnake
{
	public class VRCameraRig : SingletonMonoBehaviour<VRCameraRig>, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public new Camera camera;
		public Transform trackingSpaceTrs;
		public Transform eyesTrs;
		public Transform leftHandTrs;
		public Transform rightHandTrs;
		public Transform trs;
		static Transform currentHand;
		public static Transform CurrentHand
		{
			get
			{
				return currentHand;
			}
			set
			{
				currentHand = value;
			}
		}
		public float lookRate;
		public LayerMask whatICollideWith;
		[HideInInspector]
		public float cameraDistance;
		[HideInInspector]
		public Vector3 positionOffset;
		Quaternion rota;
		bool setOrientationInput;
		bool previousSetOrientationInput;
		
		void Start ()
		{
			CurrentHand = rightHandTrs;
			cameraDistance = trs.localPosition.magnitude;
			positionOffset = trs.localPosition;
			trs.SetParent(null);
			SetOrientation ();
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			setOrientationInput = InputManager.SetOrientationInput;
			if (InputManager.Instance.inputDevice == InputManager.InputDevice.OculusRift || InputManager.Instance.inputDevice == InputManager.InputDevice.OculusQuest)
				UpdateAnchors ();
			else
			{
				Vector2 rotaInput = Mouse.current.delta.ReadValue().FlipY() * lookRate * Time.deltaTime;
				if (rotaInput != Vector2.zero)
				{
					trackingSpaceTrs.RotateAround(trackingSpaceTrs.position, trackingSpaceTrs.right, rotaInput.y);
					trackingSpaceTrs.RotateAround(trackingSpaceTrs.position, Vector3.up, rotaInput.x);
				}
				RaycastHit hit;
				if (Physics.Raycast(Snake.instance.HeadPosition, trs.position - Snake.instance.HeadPosition, out hit, cameraDistance, whatICollideWith))
					positionOffset = positionOffset.normalized * hit.distance;
				else
					positionOffset = positionOffset.normalized * cameraDistance;
				trs.position = Snake.instance.HeadPosition + (rota * positionOffset);
			}
			if (setOrientationInput && (!previousSetOrientationInput || InputManager._InputDevice == InputManager.InputDevice.KeyboardAndMouse))
				SetOrientation ();
			previousSetOrientationInput = setOrientationInput;
		}

		void OnDestroy ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		void UpdateAnchors ()
		{
			if (Snake.instance != null)
			{
				RaycastHit hit;
				if (Physics.Raycast(Snake.instance.HeadPosition, trs.position - Snake.instance.HeadPosition, out hit, cameraDistance, whatICollideWith))
					positionOffset = positionOffset.normalized * hit.distance;
				else
					positionOffset = positionOffset.normalized * cameraDistance;
				trs.position = Snake.instance.HeadPosition + (rota * positionOffset);
			}
			eyesTrs.localPosition = InputManager.HeadPosition;
			eyesTrs.localRotation = InputManager.HeadRotation;
			leftHandTrs.localPosition = InputManager.LeftHandPosition;
			leftHandTrs.localRotation = InputManager.LeftHandRotation;
			rightHandTrs.localPosition = InputManager.RightHandPosition;
			rightHandTrs.localRotation = InputManager.RightHandRotation;
		}
		
		void SetOrientation ()
		{
			rota = eyesTrs.rotation;
			if (InputManager._InputDevice != InputManager.InputDevice.KeyboardAndMouse)
				trackingSpaceTrs.forward = eyesTrs.forward.GetXZ();
		}
	}
}