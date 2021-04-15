using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.Oculus.Input;
using UnityEngine.InputSystem;
using Extensions;

namespace AmbitiousSnake
{
	public class InputManager : SingletonMonoBehaviour<InputManager>, IUpdatable
	{
		public InputDevice inputDevice;
		public static InputDevice _InputDevice
		{
			get
			{
				return Instance.inputDevice;
			}
		}
		public InputSettings settings;
		public static InputSettings Settings
		{
			get
			{
				return Instance.settings;
			}
		}
		public static bool UsingGamepad
		{
			get
			{
				return Gamepad.current != null;
			}
		}
		public static bool UsingMouse
		{
			get
			{
				return Mouse.current != null;
			}
		}
		public static bool UsingKeyboard
		{
			get
			{
				return Keyboard.current != null;
			}
		}
		public static bool LeftClickInput
		{
			get
			{
				if (UsingGamepad)
					return false;
				else
					return Mouse.current.leftButton.isPressed;
			}
		}
		public bool _LeftClickInput
		{
			get
			{
				return LeftClickInput;
			}
		}
		public static Vector2 MousePosition
		{
			get
			{
				if (UsingMouse)
					return Mouse.current.position.ReadValue();
				else
					return VectorExtensions.NULL3;
			}
		}
		public Vector2 _MousePosition
		{
			get
			{
				return MousePosition;
			}
		}
		public static bool SubmitInput
		{
			get
			{
				if (UsingGamepad)
					return Gamepad.current.aButton.isPressed;
				else
					return Keyboard.current.enterKey.isPressed;// || Mouse.current.leftButton.isPressed;
			}
		}
		public bool _SubmitInput
		{
			get
			{
				return SubmitInput;
			}
		}
		public static Vector2 UIMovementInput
		{
			get
			{
				if (UsingGamepad)
					return Vector2.ClampMagnitude(Gamepad.current.leftStick.ReadValue(), 1);
				else
				{
					int x = 0;
					if (Keyboard.current.dKey.isPressed)
						x ++;
					if (Keyboard.current.aKey.isPressed)
						x --;
					int y = 0;
					if (Keyboard.current.wKey.isPressed)
						y ++;
					if (Keyboard.current.sKey.isPressed)
						y --;
					return Vector2.ClampMagnitude(new Vector2(x, y), 1);
				}
			}
		}
		public Vector2 _UIMovementInput
		{
			get
			{
				return UIMovementInput;
			}
		}
		public static bool SkipCinematicInput
		{
			get
			{
				if (_InputDevice != InputDevice.KeyboardAndMouse)
					return RightTouchController.primaryButton.isPressed;
				else
					return Keyboard.current.spaceKey.isPressed;
			}
		}
		public bool _SkipCinematicInput
		{
			get
			{
				return SkipCinematicInput;
			}
		}
		public static bool LeftMoveInput
		{
			get
			{
				return LeftGripInput;
			}
		}
		public bool _LeftMoveInput
		{
			get
			{
				return LeftMoveInput;
			}
		}
		public static bool RightMoveInput
		{
			get
			{
				return RightGripInput;
			}
		}
		public bool _RightMoveInput
		{
			get
			{
				return RightMoveInput;
			}
		}
		public static Vector3 MoveInput
		{
			get
			{
				Vector3 output = Vector3.zero;
				if (_InputDevice == InputDevice.KeyboardAndMouse && Keyboard.current.wKey.isPressed)
					output = Quaternion.Inverse(VRCameraRig.instance.eyesTrs.rotation) * VRCameraRig.instance.eyesTrs.forward;
				if (!GameManager.gameModifierDict["Move hand to change direction"].isActive)
				{
					if (LeftMoveInput)
						output = LeftHandRotation * Vector3.forward;
					if (RightMoveInput)
						output += RightHandRotation * Vector3.forward;
				}
				else
				{
					if (LeftMoveInput)
						output = VRCameraRig.instance.leftHandTrs.position - instance.leftHandTrsAtBeginMove.position;
					if (RightMoveInput)
						output += VRCameraRig.instance.rightHandTrs.position - instance.rightHandTrsAtBeginMove.position;
					output = Quaternion.Inverse(VRCameraRig.instance.eyesTrs.rotation) * output.normalized;
				}
				return Vector3.ClampMagnitude(output, 1);
			}
		}
		public Vector3 _MoveInput
		{
			get
			{
				return MoveInput;
			}
		}
		public static float ChangeLengthInput
		{
			get
			{
				float output = 0;
				if (_InputDevice == InputDevice.KeyboardAndMouse)
					output = Mouse.current.leftButton.isPressed.GetHashCode() - Mouse.current.rightButton.isPressed.GetHashCode();
				if (LeftTouchController != null)
					// output = LeftTouchController.gripPressed.isPressed.GetHashCode();
					output = LeftTouchController.thumbstick.ReadValue().y;
				if (RightTouchController != null)
					// output -= RightTouchController.gripPressed.isPressed.GetHashCode();
					output += RightTouchController.thumbstick.ReadValue().y;
				output = Mathf.Clamp(output, -1, 1);
				if (Mathf.Abs(output) <= Settings.defaultDeadzoneMin)
					output = 0;
				return output;
			}
		}
		public float _ChangeLengthInput
		{
			get
			{
				return ChangeLengthInput;
			}
		}
		public static bool SetOrientationInput
		{
			get
			{
				if (_InputDevice == InputDevice.KeyboardAndMouse)
					return Keyboard.current.spaceKey.isPressed;
				else
					return LeftThumbstickClickedInput || RightThumbstickClickedInput;
			}
		}
		public bool _SetOrientationInput
		{
			get
			{
				return SetOrientationInput;
			}
		}
		public static bool RestartInput
		{
			get
			{
#if UNITY_EDITOR
				return Keyboard.current.rKey.isPressed;
#endif
				if (_InputDevice == InputDevice.KeyboardAndMouse)
					return Keyboard.current.rKey.isPressed;
				else
					return false;
			}
		}
		public bool _RestartInput
		{
			get
			{
				return RestartInput;
			}
		}
		public static bool LeftGameplayMenuInput
		{
			get
			{
				if (_InputDevice == InputDevice.KeyboardAndMouse)
					return Keyboard.current.leftArrowKey.isPressed;
				else
					return LeftTouchController != null && (LeftTouchController.primaryButton.isPressed || LeftTouchController.secondaryButton.isPressed || LeftTouchController.triggerPressed.isPressed);
			}
		}
		public bool _LeftGameplayMenuInput
		{
			get
			{
				return LeftGameplayMenuInput;
			}
		}
		public static bool RightGameplayMenuInput
		{
			get
			{
				if (_InputDevice == InputDevice.KeyboardAndMouse)
					return Keyboard.current.rightArrowKey.isPressed;
				else
					return RightTouchController != null && (RightTouchController.primaryButton.isPressed || RightTouchController.secondaryButton.isPressed || RightTouchController.triggerPressed.isPressed);
			}
		}
		public bool _RightGameplayMenuInput
		{
			get
			{
				return RightGameplayMenuInput;
			}
		}
		public static bool GameplayMenuInput
		{
			get
			{
				if (_InputDevice == InputDevice.KeyboardAndMouse)
					return Keyboard.current.escapeKey.isPressed;
				else
					return LeftGameplayMenuInput || RightGameplayMenuInput;
			}
		}
		public bool _GameplayMenuInput
		{
			get
			{
				return GameplayMenuInput;
			}
		}
		public static bool LeftGripInput
		{
			get
			{
				return LeftTouchController != null && LeftTouchController.gripPressed.isPressed;
			}
		}
		public bool _LeftGripInput
		{
			get
			{
				return LeftGripInput;
			}
		}
		public static bool RightGripInput
		{
			get
			{
				return RightTouchController != null && RightTouchController.gripPressed.isPressed;
			}
		}
		public bool _RightGripInput
		{
			get
			{
				return RightGripInput;
			}
		}
		public static bool LeftTriggerInput
		{
			get
			{
				return LeftTouchController != null && LeftTouchController.triggerPressed.isPressed;
			}
		}
		public bool _LeftTriggerInput
		{
			get
			{
				return LeftTriggerInput;
			}
		}
		public static bool RightTriggerInput
		{
			get
			{
				return RightTouchController != null && RightTouchController.triggerPressed.isPressed;
			}
		}
		public bool _RightTriggerInput
		{
			get
			{
				return RightTriggerInput;
			}
		}
		public static bool LeftThumbstickClickedInput
		{
			get
			{
				return LeftTouchController != null && LeftTouchController.thumbstickClicked.isPressed;
			}
		}
		public bool _LeftThumbstickClickedInput
		{
			get
			{
				return LeftThumbstickClickedInput;
			}
		}
		public static bool RightThumbstickClickedInput
		{
			get
			{
				return RightTouchController != null && RightTouchController.thumbstickClicked.isPressed;
			}
		}
		public bool _RightThumbstickClickedInput
		{
			get
			{
				return RightThumbstickClickedInput;
			}
		}
		public static Vector3 HeadPosition
		{
			get
			{
				if (Hmd != null)
					return Hmd.devicePosition.ReadValue();
				else
					return Vector3.zero;
			}
		}
		public Vector3 _HeadPosition
		{
			get
			{
				return HeadPosition;
			}
		}
		public static Quaternion HeadRotation
		{
			get
			{
				if (Hmd != null)
					return Hmd.deviceRotation.ReadValue();
				else
					return QuaternionExtensions.NULL;
			}
		}
		public Quaternion _HeadRotation
		{
			get
			{
				return HeadRotation;
			}
		}
		public static Vector3 LeftHandPosition
		{
			get
			{
				if (LeftTouchController != null)
					return LeftTouchController.devicePosition.ReadValue();
				else
					return VectorExtensions.NULL3;
			}
		}
		public Vector3 _LeftHandPosition
		{
			get
			{
				return LeftHandPosition;
			}
		}
		public static Quaternion LeftHandRotation
		{
			get
			{
				if (LeftTouchController != null)
					return LeftTouchController.deviceRotation.ReadValue();
				else
					return QuaternionExtensions.NULL;
			}
		}
		public Quaternion _LeftHandRotation
		{
			get
			{
				return LeftHandRotation;
			}
		}
		public static Vector3 RightHandPosition
		{
			get
			{
				if (RightTouchController != null)
					return RightTouchController.devicePosition.ReadValue();
				else
					return VectorExtensions.NULL3;
			}
		}
		public Vector3 _RightHandPosition
		{
			get
			{
				return RightHandPosition;
			}
		}
		public static Quaternion RightHandRotation
		{
			get
			{
				if (RightTouchController != null)
					return RightTouchController.deviceRotation.ReadValue();
				else
					return QuaternionExtensions.NULL;
			}
		}
		public Quaternion _RightHandRotation
		{
			get
			{
				return RightHandRotation;
			}
		}
		public static OculusHMD Hmd
		{
			get
			{
				return InputSystem.GetDevice<OculusHMD>();
			}
		}
		public static OculusTouchController LeftTouchController
		{
			get
			{
				return (OculusTouchController) OculusTouchController.leftHand;
			}
		}
		public static OculusTouchController RightTouchController
		{
			get
			{
				return (OculusTouchController) OculusTouchController.rightHand;
			}
		}
		public static bool leftMoveInput;
		public static bool previousLeftMoveInput;
		public static bool rightMoveInput;
		public static bool previousRightMoveInput;
		public Transform trs;
		public Transform leftHandTrsAtBeginMove;
		public Transform rightHandTrsAtBeginMove;

		public override void Awake ()
		{
			base.Awake ();
			trs.SetParent(null);
			leftHandTrsAtBeginMove.SetParent(VRCameraRig.Instance.trs);
			rightHandTrsAtBeginMove.SetParent(VRCameraRig.instance.trs);
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			leftMoveInput = LeftMoveInput;
			rightMoveInput = RightMoveInput;
			if (GameManager.gameModifierDict["Move hand to change direction"].isActive)
			{
				if (!previousLeftMoveInput && leftMoveInput)
				{
					leftHandTrsAtBeginMove.position = VRCameraRig.instance.leftHandTrs.position;
					leftHandTrsAtBeginMove.gameObject.SetActive(true);
				}
				else if (previousLeftMoveInput && !leftMoveInput)
					leftHandTrsAtBeginMove.gameObject.SetActive(false);
				if (!previousRightMoveInput && rightMoveInput)
				{
					rightHandTrsAtBeginMove.position = VRCameraRig.instance.rightHandTrs.position;
					rightHandTrsAtBeginMove.gameObject.SetActive(true);
				}
				else if (previousRightMoveInput && !rightMoveInput)
					rightHandTrsAtBeginMove.gameObject.SetActive(false);
			}
			else
			{
				leftHandTrsAtBeginMove.gameObject.SetActive(false);
				rightHandTrsAtBeginMove.gameObject.SetActive(false);
			}
			previousLeftMoveInput = leftMoveInput;
			previousRightMoveInput = rightMoveInput;
		}

		void OnDestroy ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		public static float GetAxis (InputControl<float> positiveButton, InputControl<float> negativeButton)
		{
			return positiveButton.ReadValue() - negativeButton.ReadValue();
		}

		public static Vector2 GetAxis2D (InputControl<float> positiveXButton, InputControl<float> negativeXButton, InputControl<float> positiveYButton, InputControl<float> negativeYButton)
		{
			Vector2 output = new Vector2();
			output.x = positiveXButton.ReadValue() - negativeXButton.ReadValue();
			output.y = positiveYButton.ReadValue() - negativeYButton.ReadValue();
			output = Vector2.ClampMagnitude(output, 1);
			return output;
		}
		
		public enum HotkeyState
		{
			Down,
			Held,
			Up
		}
		
		public enum InputDevice
		{
			OculusQuest,
			OculusGo,
			OculusRift,
			KeyboardAndMouse
		}
	}
}