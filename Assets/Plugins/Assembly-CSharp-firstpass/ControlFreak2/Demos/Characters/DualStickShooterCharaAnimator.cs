using UnityEngine;

namespace ControlFreak2.Demos.Characters
{
	public class DualStickShooterCharaAnimator : MonoBehaviour
	{
		public DualStickShooterCharaMotor charaMotor;

		public Animator animator;

		public string forwardBackwardParamName = "Vertical";

		public string strafeParamName = "Horizontal";

		public string speedParamName = "Speed";

		private void OnEnable()
		{
			if (animator == null)
			{
				animator = GetComponent<Animator>();
			}
			if (charaMotor == null)
			{
				charaMotor = GetComponent<DualStickShooterCharaMotor>();
			}
		}

		private void Update()
		{
			if (!(animator == null) && !(charaMotor == null))
			{
				Vector2 vector = new Vector2(charaMotor.GetLocalDir().x, charaMotor.GetLocalDir().z);
				if (!string.IsNullOrEmpty(speedParamName))
				{
					animator.SetFloat(speedParamName, vector.magnitude);
				}
				if (!string.IsNullOrEmpty(forwardBackwardParamName))
				{
					animator.SetFloat(forwardBackwardParamName, vector.y);
				}
				if (!string.IsNullOrEmpty(strafeParamName))
				{
					animator.SetFloat(strafeParamName, vector.x);
				}
			}
		}
	}
}
