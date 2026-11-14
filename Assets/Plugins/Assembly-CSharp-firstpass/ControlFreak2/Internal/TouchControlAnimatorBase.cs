using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[ExecuteInEditMode]
	public abstract class TouchControlAnimatorBase : ComponentBase
	{
		public bool autoConnectToSource;

		public TouchControl sourceControl;

		protected Type sourceType;

		public TouchControlAnimatorBase(Type sourceType)
		{
			this.sourceType = sourceType;
		}

		protected abstract void OnUpdateAnimator(bool skipAnim);

		public void UpdateAnimator(bool skipAnim)
		{
			if (!(sourceControl == null))
			{
				OnUpdateAnimator(skipAnim);
			}
		}

		public void SetSourceControl(TouchControl c)
		{
			if (sourceControl != null)
			{
				sourceControl.RemoveAnimator(this);
			}
			if (c != null && !c.CanBeUsed())
			{
				c = null;
			}
			sourceControl = c;
			if (sourceControl != null)
			{
				sourceControl.AddAnimator(this);
			}
		}

		public Type GetSourceControlType()
		{
			return sourceType;
		}

		public TouchControl FindAutoSource()
		{
			return (TouchControl)GetComponentInParent(sourceType);
		}

		public void AutoConnectToSource()
		{
			TouchControl touchControl = FindAutoSource();
			if (touchControl != null)
			{
				SetSourceControl(touchControl);
			}
		}

		public bool IsIllegallyAttachedToSource()
		{
			if (sourceControl != null)
			{
				return sourceControl.gameObject == base.gameObject;
			}
			return false;
		}

		public void InvalidateHierarchy()
		{
			if (autoConnectToSource && sourceControl == null)
			{
				SetSourceControl(FindAutoSource());
			}
		}

		protected override void OnInitComponent()
		{
			if (autoConnectToSource || sourceControl == null)
			{
				SetSourceControl(FindAutoSource());
			}
		}

		protected override void OnDestroyComponent()
		{
		}

		protected override void OnEnableComponent()
		{
		}

		protected override void OnDisableComponent()
		{
		}
	}
}
