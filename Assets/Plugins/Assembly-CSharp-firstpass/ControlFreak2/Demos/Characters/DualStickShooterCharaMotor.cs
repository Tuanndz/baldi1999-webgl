using System;
using UnityEngine;

namespace ControlFreak2.Demos.Characters
{
	public class DualStickShooterCharaMotor : MonoBehaviour
	{
		public enum WorldPlane
		{
			XZ = 0,
			XY = 1,
			ZY = 2
		}

		public Transform cameraTransform;

		public string moveHorzAxis = "Horizontal";

		public string moveVertAxis = "Vertical";

		public string aimHorzAxis = "Horizontal2";

		public string aimVertAxis = "Vertical2";

		private Rigidbody rb;

		public float forwardSpeed = 10f;

		public float strafeSpeed = 6f;

		public float backwardSpeed = 4f;

		public float turnSpeed = 720f;

		public float orientationOffset;

		public Vector3 gravity = new Vector3(0f, -0.5f, 0f);

		private Vector3 worldVel;

		private Vector3 localDir;

		private float moveTilt;

		private float worldMoveAngle;

		private float worldOrientAngle;

		private float inputMoveAngle;

		private float inputMoveTilt;

		private float inputAimAngle;

		private float inputAimTilt;

		private void OnEnable()
		{
			worldOrientAngle = (worldMoveAngle = GetAngleFromDir(base.transform.forward, WorldPlane.XZ, 0f));
			rb = GetComponent<Rigidbody>();
			MoveOrientation(worldOrientAngle);
		}

		private void MoveOrientation(float angle)
		{
			Quaternion quaternion = Quaternion.AngleAxis(angle + orientationOffset, Vector3.up);
			if (rb != null)
			{
				rb.MoveRotation(quaternion);
			}
			else
			{
				base.transform.rotation = quaternion;
			}
		}

		private void MovePosition(Vector3 worldPos)
		{
			if (rb != null)
			{
				rb.MovePosition(worldPos);
			}
			else
			{
				base.transform.position = worldPos;
			}
		}

		private void Update()
		{
			SetCameraRelativeInput(cameraTransform, string.IsNullOrEmpty(moveHorzAxis) ? 0f : CF2Input.GetAxis(moveHorzAxis), string.IsNullOrEmpty(moveVertAxis) ? 0f : CF2Input.GetAxis(moveVertAxis), string.IsNullOrEmpty(aimHorzAxis) ? 0f : CF2Input.GetAxis(aimHorzAxis), string.IsNullOrEmpty(aimVertAxis) ? 0f : CF2Input.GetAxis(aimVertAxis));
		}

		private void FixedUpdate()
		{
			float num = 0.01f;
			float target = 0f;
			float num2 = 0f;
			if (inputAimTilt > num)
			{
				target = inputAimAngle;
				num2 = inputAimTilt;
			}
			else if (inputMoveTilt > num)
			{
				target = inputMoveAngle;
				num2 = inputMoveTilt;
			}
			if (num2 > 0f)
			{
				worldOrientAngle = Mathf.MoveTowardsAngle(worldOrientAngle, target, num2 * turnSpeed * Time.deltaTime);
			}
			if (inputMoveTilt > num)
			{
				worldMoveAngle = inputMoveAngle;
				moveTilt = inputMoveTilt;
			}
			else
			{
				moveTilt = 0f;
			}
			float num3 = worldMoveAngle - worldOrientAngle;
			localDir = Quaternion.AngleAxis(num3, Vector3.up) * new Vector3(0f, 0f, 1f);
			localDir *= moveTilt;
			Vector3 vector = localDir;
			vector.x *= strafeSpeed;
			vector.z *= ((localDir.z >= 0f) ? forwardSpeed : backwardSpeed);
			worldVel = Quaternion.AngleAxis(0f - num3 + worldMoveAngle, Vector3.up) * vector;
			Vector3 position = base.transform.position;
			position += worldVel * Time.deltaTime;
			position += gravity * Time.deltaTime;
			MovePosition(position);
			MoveOrientation(worldOrientAngle);
		}

		public float GetOrientAngle()
		{
			return worldOrientAngle;
		}

		public Vector3 GetLocalDir()
		{
			return localDir;
		}

		public void SetWorldInput(float moveAngle, float moveTilt, float aimAngle, float aimTilt)
		{
			inputMoveAngle = moveAngle;
			inputMoveTilt = moveTilt;
			inputAimAngle = aimAngle;
			inputAimTilt = aimTilt;
		}

		public void SetCameraRelativeInput(Transform camTr, float moveX, float moveY, float aimX, float aimY)
		{
			float num = 0f;
			float tilt = 0f;
			float num2 = 0f;
			float tilt2 = 0f;
			num = GetJoyAngleAndTilt(camTr, moveX, moveY, WorldPlane.XZ, out tilt);
			num2 = GetJoyAngleAndTilt(camTr, aimX, aimY, WorldPlane.XZ, out tilt2);
			SetWorldInput(num, tilt, num2, tilt2);
		}

		public static float GetAngleFromDir(Vector3 dir, WorldPlane plane, float fallbackAngle)
		{
			Vector2 vector;
			switch (plane)
			{
			default:
				vector = new Vector2(dir.z, dir.y);
				break;
			case WorldPlane.XZ:
				vector = new Vector2(dir.x, dir.z);
				break;
			case WorldPlane.XY:
				vector = new Vector2(dir.x, dir.y);
				break;
			}
			Vector2 vector2 = vector;
			if (vector2.sqrMagnitude < 1E-06f)
			{
				return fallbackAngle;
			}
			vector2.Normalize();
			return 57.29578f * Mathf.Atan2(vector2.x, vector2.y);
		}

		public static Vector3 GetWorldVecFromCamera(Transform camTr, float x, float y, WorldPlane plane)
		{
			Vector3 vector;
			Vector3 vector2;
			switch (plane)
			{
			case WorldPlane.XZ:
				vector = camTr.right;
				vector2 = camTr.forward;
				vector.y = vector.z;
				vector2.y = vector2.z;
				break;
			case WorldPlane.XY:
				vector = camTr.right;
				vector2 = camTr.up;
				vector.z = 0f;
				vector2.z = 0f;
				break;
			default:
				vector = camTr.forward;
				vector2 = camTr.up;
				vector.x = vector.z;
				vector2.x = vector2.z;
				break;
			}
			vector.z = 0f;
			vector2.z = 0f;
			vector.Normalize();
			vector2.Normalize();
			return vector * x + vector2 * y;
		}

		public static float GetJoyAngleAndTilt(Transform camTr, float x, float y, WorldPlane plane, out float tilt)
		{
			if (new Vector2(x, y).sqrMagnitude < 1E-06f)
			{
				tilt = 0f;
				return 0f;
			}
			Vector2 vector = GetWorldVecFromCamera(camTr, x, y, plane);
			float magnitude = vector.magnitude;
			if (magnitude < 0.0001f)
			{
				tilt = 0f;
				return 0f;
			}
			vector /= magnitude;
			tilt = Mathf.Min(magnitude, 1f);
			return 57.29578f * Mathf.Atan2(vector.x, vector.y);
		}

		public static Vector3 AngleToWorldVec(float angle, WorldPlane plane)
		{
			angle *= (float)Math.PI / 180f;
			float num = Mathf.Sin(angle);
			float num2 = Mathf.Cos(angle);
			switch (plane)
			{
			case WorldPlane.XY:
				return new Vector3(num, num2, 0f);
			case WorldPlane.XZ:
				return new Vector3(num, 0f, num2);
			default:
				return new Vector3(0f, num2, num);
			}
		}

		public static Vector3 RotateVecOnPlane(Vector3 v, float angle, WorldPlane plane)
		{
			angle *= (float)Math.PI / 180f;
			float num = Mathf.Sin(angle);
			float num2 = Mathf.Cos(angle);
			switch (plane)
			{
			case WorldPlane.XY:
				return new Vector3(v.x * num2 - v.y * num, v.x * num + v.y * num2, v.z);
			case WorldPlane.XZ:
				return new Vector3(v.x * num2 - v.z * num, v.y, v.x * num + v.z * num2);
			default:
				return new Vector3(v.x, v.z * num + v.y * num2, v.z * num2 - v.y * num);
			}
		}
	}
}
