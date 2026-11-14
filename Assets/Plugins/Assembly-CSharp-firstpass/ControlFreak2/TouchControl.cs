using System;
using System.Collections.Generic;
using ControlFreak2.Internal;
using UnityEngine;

namespace ControlFreak2
{
	[RequireComponent(typeof(RectTransform))]
	public abstract class TouchControl : ComponentBase, IBindingContainer
	{
		public enum Shape
		{
			Rectangle = 0,
			Square = 1,
			Circle = 2,
			Ellipse = 3
		}

		public enum SwipeOffMode
		{
			Disabled = 0,
			Enabled = 1,
			OnlyIfSwipedOver = 2
		}

		public enum SwipeOverOthersMode
		{
			Disabled = 0,
			Enabled = 1,
			OnlyIfTouchedDirectly = 2
		}

		public enum TouchStartType
		{
			DirectPress = 0,
			ProxyPress = 1,
			SwipeOver = 2
		}

		public enum TouchEndType
		{
			Release = 0,
			Cancel = 1,
			SwipeOff = 2
		}

		public class Hit
		{
			public TouchControl c;

			public float depth;

			public bool indirectHit;

			public Vector2 localPos;

			public Vector2 closestLocalPos;

			public Vector2 screenPos;

			public Vector2 closestScreenPos;

			public float screenDistSqPx;

			public void Reset()
			{
				c = null;
			}

			public bool IsEmpty()
			{
				return c == null;
			}

			public void CopyFrom(Hit b)
			{
				c = b.c;
				depth = b.depth;
				indirectHit = b.indirectHit;
				localPos = b.localPos;
				closestLocalPos = b.closestLocalPos;
				screenPos = b.screenPos;
				closestScreenPos = b.closestScreenPos;
				screenDistSqPx = b.screenDistSqPx;
			}

			public bool IsHigherThan(Hit r)
			{
				if (c == null != (r.c == null))
				{
					return c != null;
				}
				if (Mathf.RoundToInt(depth) <= Mathf.RoundToInt(r.depth) && !(c is DynamicRegion) && r.c is DynamicRegion)
				{
					return true;
				}
				if (indirectHit != r.indirectHit)
				{
					return !indirectHit;
				}
				if (Mathf.RoundToInt(depth) != Mathf.RoundToInt(r.depth))
				{
					return depth < r.depth;
				}
				if (screenDistSqPx != r.screenDistSqPx)
				{
					return screenDistSqPx < r.screenDistSqPx;
				}
				return false;
			}
		}

		public class HitPool : ObjectPoolBase<Hit>
		{
			public delegate bool TouchControlFilterFunc(TouchControl c);

			private Hit tempHit;

			public HitPool()
			{
				tempHit = new Hit();
			}

			public bool HitTestAny(List<TouchControl> controlList, Vector2 screenPos, Camera cam, float fingerRadPx = 0f, TouchControlFilterFunc filter = null)
			{
				EnsureCapacity(1);
				Clear();
				for (int i = 0; i < controlList.Count; i++)
				{
					TouchControl touchControl = controlList[i];
					if (!(touchControl == null) && (filter == null || filter(touchControl)) && touchControl.HitTest(screenPos, cam, fingerRadPx, tempHit))
					{
						GetNewObject().CopyFrom(tempHit);
						return true;
					}
				}
				return false;
			}

			public int HitTest(List<TouchControl> controlList, Vector2 screenPos, Camera cam, int maxHits = 8, float fingerRadPx = 0f, TouchControlFilterFunc filter = null)
			{
				if (maxHits < 1)
				{
					maxHits = 1;
				}
				Clear();
				EnsureCapacity(maxHits);
				for (int i = 0; i < controlList.Count; i++)
				{
					TouchControl touchControl = controlList[i];
					if (touchControl == null || (filter != null && !filter(touchControl)) || !touchControl.HitTest(screenPos, cam, fingerRadPx, tempHit))
					{
						continue;
					}
					int num = -1;
					for (int j = 0; j < GetList().Count; j++)
					{
						if (tempHit.IsHigherThan(GetList()[j]))
						{
							num = j;
							break;
						}
					}
					if (GetUsedCount() < maxHits)
					{
						GetNewObject(num).CopyFrom(tempHit);
					}
					else if (num >= 0)
					{
						Hit hit = GetList()[maxHits - 1];
						GetList().RemoveAt(maxHits - 1);
						GetList().Insert(num, hit);
						hit.CopyFrom(tempHit);
					}
				}
				return GetUsedCount();
			}

			protected override Hit CreateInternalObject()
			{
				return new Hit();
			}
		}

		public bool ignoreFingerRadius;

		public bool cantBeControlledDirectly;

		public bool shareTouch;

		public bool dontAcceptSharedTouches;

		public bool canBeSwipedOver;

		public bool restictSwipeOverTargets;

		public SwipeOverOthersMode swipeOverOthersMode = SwipeOverOthersMode.OnlyIfTouchedDirectly;

		public SwipeOffMode swipeOffMode = SwipeOffMode.OnlyIfSwipedOver;

		public List<TouchControl> swipeOverTargetList;

		public Shape shape;

		[NonSerialized]
		protected List<TouchControlAnimatorBase> animatorList;

		public DisablingConditionSet disablingConditions;

		[NonSerialized]
		private int hidingFlagsCur;

		[NonSerialized]
		private InputRig _rig;

		[NonSerialized]
		private Canvas _canvas;

		[NonSerialized]
		private TouchControlPanel _panel;

		protected const int HIDDEN_BY_USER = 0;

		protected const int HIDDEN_BY_DISABLED_GO = 1;

		protected const int HIDDEN_BY_CONDITIONS = 4;

		protected const int HIDDEN_BY_RIG = 5;

		protected const int HIDDEN_DUE_TO_INACTIVITY = 6;

		protected const int HIDDEN_AND_DISABLED_MASK = -65;

		[NonSerialized]
		private bool isHidden;

		[NonSerialized]
		private bool baseAlphaAnimOn;

		[NonSerialized]
		private float baseAlphaStart;

		[NonSerialized]
		private float baseAlphaEnd;

		[NonSerialized]
		private float baseAlphaCur;

		[NonSerialized]
		private float baseAlphaAnimDur;

		[NonSerialized]
		private float baseAlphaAnimElapsed;

		public InputRig rig
		{
			get
			{
				return _rig;
			}
			protected set
			{
				_rig = value;
			}
		}

		public Canvas canvas
		{
			get
			{
				return _canvas;
			}
			protected set
			{
				_canvas = value;
			}
		}

		public TouchControlPanel panel
		{
			get
			{
				return _panel;
			}
			protected set
			{
				_panel = value;
			}
		}

		public TouchControl()
		{
			disablingConditions = new DisablingConditionSet(null);
			animatorList = new List<TouchControlAnimatorBase>(2);
			swipeOverTargetList = new List<TouchControl>(2);
		}

		protected abstract void OnInitControl();

		protected abstract void OnUpdateControl();

		protected abstract void OnDestroyControl();

		public void GetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
			OnGetSubBindingDescriptions(descList, undoObject, parentMenuPath);
		}

		public bool IsBoundToAxis(string axisName, InputRig rig)
		{
			return OnIsBoundToAxis(axisName, rig);
		}

		public bool IsBoundToKey(KeyCode key, InputRig rig)
		{
			return OnIsBoundToKey(key, rig);
		}

		public bool IsEmulatingTouches()
		{
			return OnIsEmulatingTouches();
		}

		public bool IsEmulatingMousePosition()
		{
			return OnIsEmulatingMousePosition();
		}

		public virtual bool IsUsingKeyForEmulation(KeyCode key)
		{
			return false;
		}

		protected virtual void OnGetSubBindingDescriptions(BindingDescriptionList descList, UnityEngine.Object undoObject, string parentMenuPath)
		{
		}

		protected virtual bool OnIsBoundToAxis(string axisName, InputRig rig)
		{
			return false;
		}

		protected virtual bool OnIsBoundToKey(KeyCode key, InputRig rig)
		{
			return false;
		}

		protected virtual bool OnIsEmulatingTouches()
		{
			return false;
		}

		protected virtual bool OnIsEmulatingMousePosition()
		{
			return false;
		}

		public abstract void ResetControl();

		public abstract void ReleaseAllTouches();

		public abstract bool OnTouchStart(TouchObject touch, TouchControl sender, TouchStartType touchStartType);

		public abstract bool OnTouchEnd(TouchObject touch, TouchEndType touchEndType);

		public abstract bool OnTouchMove(TouchObject touch);

		public abstract bool OnTouchPressureChange(TouchObject touch);

		protected bool CheckSwipeOff(TouchObject touchObj, TouchStartType touchStartType)
		{
			if (touchObj == null || swipeOffMode == SwipeOffMode.Disabled || (swipeOffMode == SwipeOffMode.OnlyIfSwipedOver && touchStartType != TouchStartType.SwipeOver))
			{
				return false;
			}
			if (!RaycastScreen(touchObj.screenPosCur, touchObj.cam))
			{
				touchObj.ReleaseControl(this, TouchEndType.SwipeOff);
				return true;
			}
			return false;
		}

		public virtual bool CanShareTouchWith(TouchControl c)
		{
			return true;
		}

		public abstract bool CanBeSwipedOverFromNothing(TouchObject touchObj);

		protected bool CanBeSwipedOverFromNothingDefault(TouchObject touchObj)
		{
			if (canBeSwipedOver)
			{
				return IsActive();
			}
			return false;
		}

		public abstract bool CanBeSwipedOverFromRestrictedList(TouchObject touchObj);

		protected bool CanBeSwipedOverFromRestrictedListDefault(TouchObject touchObj)
		{
			return IsActive();
		}

		public abstract bool CanSwipeOverOthers(TouchObject touchObj);

		protected bool CanSwipeOverOthersDefault(TouchObject touchObj, TouchObject myTouchObj, TouchStartType touchStartType)
		{
			if (myTouchObj == touchObj)
			{
				if (swipeOverOthersMode != SwipeOverOthersMode.Enabled)
				{
					if (swipeOverOthersMode == SwipeOverOthersMode.OnlyIfTouchedDirectly)
					{
						return touchStartType != TouchStartType.SwipeOver;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public virtual bool CanBeTouchedDirectly(TouchObject touchObj)
		{
			if (!cantBeControlledDirectly)
			{
				return IsActive();
			}
			return false;
		}

		public virtual bool CanBeActivatedByOtherControl(TouchControl c, TouchObject touchObj)
		{
			return IsActive();
		}

		public static int CompareByDepth(TouchControl a, TouchControl b)
		{
			if (a == null || b == null)
			{
				if (!(a == null) || !(b == null))
				{
					if (!(a == null))
					{
						return -1;
					}
					return 1;
				}
				return 0;
			}
			float z = a.transform.position.z;
			float z2 = a.transform.position.z;
			if (!(Mathf.Abs(z - z2) < 0.001f))
			{
				if (!(z < z2))
				{
					return 1;
				}
				return -1;
			}
			return 0;
		}

		public void ShowOrHideControl(bool show, bool noAnim = false)
		{
			SetHidingFlag(0, !show);
			SyncBaseAlphaToHidingConditions(noAnim);
		}

		public void ShowControl(bool noAnim = false)
		{
			ShowOrHideControl(true, noAnim);
		}

		public void HideControl(bool noAnim = false)
		{
			ShowOrHideControl(false, noAnim);
		}

		public bool IsHiddenManually()
		{
			return (hidingFlagsCur | 0) != 0;
		}

		public void ToggleVisibility(bool noAnim)
		{
			ShowOrHideControl(IsHiddenManually(), noAnim);
		}

		public void ToggleVisibility()
		{
			ShowOrHideControl(IsHiddenManually());
		}

		public bool IsActiveButInvisible()
		{
			if (IsActive())
			{
				return GetAlpha() <= 0.0001f;
			}
			return false;
		}

		public bool IsActiveAndVisible()
		{
			if (IsActive())
			{
				return GetAlpha() > 0.0001f;
			}
			return false;
		}

		public bool IsActive()
		{
			return (hidingFlagsCur & -65) == 0;
		}

		public virtual void StoreDefaultPos()
		{
		}

		public void SetHidingFlag(int flagBit, bool state)
		{
			if (flagBit >= 0 && flagBit <= 31)
			{
				hidingFlagsCur = (state ? (hidingFlagsCur | (1 << flagBit)) : (hidingFlagsCur & ~(1 << flagBit)));
				if (!IsActive())
				{
					ReleaseAllTouches();
				}
			}
		}

		public void SyncDisablingConditions(bool skipAnim)
		{
			SetHidingFlag(1, !base.enabled || !base.gameObject.activeInHierarchy);
			if (rig != null)
			{
				SetHidingFlag(4, disablingConditions.IsInEffect());
				SetHidingFlag(5, rig.AreTouchControlsHiddenManually());
			}
			SyncBaseAlphaToHidingConditions(skipAnim);
			if (skipAnim)
			{
				UpdateAnimators(skipAnim);
			}
		}

		public void AddAnimator(TouchControlAnimatorBase a)
		{
			if (!animatorList.Contains(a))
			{
				animatorList.Add(a);
			}
		}

		public void RemoveAnimator(TouchControlAnimatorBase a)
		{
			if (animatorList.Contains(a))
			{
				animatorList.Remove(a);
			}
		}

		public List<TouchControlAnimatorBase> GetAnimatorList()
		{
			return animatorList;
		}

		protected void UpdateAnimators(bool skipAnim)
		{
			for (int i = 0; i < animatorList.Count; i++)
			{
				animatorList[i].UpdateAnimator(skipAnim);
			}
		}

		private void SyncBaseAlphaToHidingConditions(bool noAnim)
		{
			float num = ((hidingFlagsCur == 0) ? 1 : 0);
			if (Mathf.Abs(num - (baseAlphaAnimOn ? baseAlphaEnd : baseAlphaCur)) > 0.001f)
			{
				StartAlphaAnim(num, (noAnim || rig == null) ? 0f : rig.controlBaseAlphaAnimDuration);
			}
		}

		private void StartAlphaAnim(float targetAlpha, float duration)
		{
			baseAlphaStart = baseAlphaCur;
			baseAlphaEnd = targetAlpha;
			if (duration <= 0.0001f)
			{
				baseAlphaAnimOn = false;
				baseAlphaCur = baseAlphaEnd;
			}
			else
			{
				baseAlphaAnimOn = true;
				baseAlphaAnimElapsed = 0f;
				baseAlphaAnimDur = duration;
			}
		}

		private void UpdateBaseAlpha()
		{
			if (baseAlphaAnimOn)
			{
				baseAlphaAnimElapsed += CFUtils.realDeltaTime;
				if (baseAlphaAnimElapsed >= baseAlphaAnimDur)
				{
					baseAlphaCur = baseAlphaEnd;
					baseAlphaAnimOn = false;
				}
				else
				{
					baseAlphaCur = Mathf.Lerp(baseAlphaStart, baseAlphaEnd, baseAlphaAnimElapsed / baseAlphaAnimDur);
				}
			}
		}

		public float GetBaseAlpha()
		{
			return baseAlphaCur;
		}

		public virtual float GetAlpha()
		{
			return baseAlphaCur;
		}

		public Camera GetCamera()
		{
			if (!(canvas != null) || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				return null;
			}
			return canvas.worldCamera;
		}

		public void SetRig(InputRig rig)
		{
			if (rig != null && !rig.CanBeUsed())
			{
				rig = null;
			}
			if (this.rig != rig)
			{
				if (this.rig != null)
				{
					this.rig.RemoveControl(this);
				}
				this.rig = rig;
				if (this.rig != null)
				{
					this.rig.AddControl(this);
				}
			}
			disablingConditions.SetRig(this.rig);
			SyncDisablingConditions(true);
		}

		public void SetTouchControlPanel(TouchControlPanel panel)
		{
			if (!(this.panel == panel))
			{
				if (this.panel != null)
				{
					this.panel.RemoveControl(this);
				}
				if (panel != null && !panel.CanBeUsed())
				{
					panel = null;
				}
				this.panel = panel;
				if (this.panel != null)
				{
					this.panel.AddControl(this);
				}
			}
		}

		public virtual void InvalidateHierarchy()
		{
			InputRig inputRig = null;
			TouchControlPanel touchControlPanel = null;
			Canvas canvas = null;
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				TouchControlPanel component = parent.GetComponent<TouchControlPanel>();
				InputRig component2 = parent.GetComponent<InputRig>();
				Canvas component3 = parent.GetComponent<Canvas>();
				if (touchControlPanel == null && component != null)
				{
					touchControlPanel = component;
				}
				if (inputRig == null && component2 != null)
				{
					inputRig = component2;
				}
				if (canvas == null && component3 != null)
				{
					canvas = component3;
				}
				if (inputRig != null && touchControlPanel != null && canvas != null)
				{
					break;
				}
				parent = parent.parent;
			}
			this.canvas = canvas;
			if (rig != inputRig)
			{
				SetRig(inputRig);
			}
			if (panel != touchControlPanel)
			{
				SetTouchControlPanel(touchControlPanel);
			}
		}

		public Vector3 GetWorldPos()
		{
			return base.transform.position;
		}

		public virtual void SetWorldPos(Vector2 pos2D)
		{
			SetWorldPosRaw(pos2D);
		}

		protected void SetWorldPosRaw(Vector2 pos2D)
		{
			Transform transform = base.transform;
			transform.position = new Vector3(pos2D.x, pos2D.y, transform.position.z);
		}

		public virtual Rect GetLocalRect()
		{
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform == null)
			{
				return new Rect(0f, 0f, 1f, 1f);
			}
			Rect rect = rectTransform.rect;
			if (shape == Shape.Circle || shape == Shape.Square)
			{
				Vector2 center = rect.center;
				float num = Mathf.Min(rect.width, rect.height);
				return new Rect(center.x - num * 0.5f, center.y - num * 0.5f, num, num);
			}
			return rect;
		}

		protected Vector3 ScreenToWorldPos(Vector2 sp, Camera cam)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform as RectTransform, sp, cam, out localPoint);
			return base.transform.TransformPoint(localPoint);
		}

		protected Vector2 WorldToLocalPos(Vector3 wp, Vector2 worldOffset)
		{
			return base.transform.worldToLocalMatrix.MultiplyPoint3x4(wp + (Vector3)worldOffset);
		}

		protected Vector2 WorldToLocalPos(Vector3 wp)
		{
			return WorldToLocalPos(wp, Vector2.zero);
		}

		protected Vector2 ScreenToLocalPos(Vector2 sp, Camera cam, Vector2 worldOffset)
		{
			return WorldToLocalPos(ScreenToWorldPos(sp, cam), worldOffset);
		}

		protected Vector2 ScreenToLocalPos(Vector2 sp, Camera cam)
		{
			return ScreenToLocalPos(sp, cam, Vector2.zero);
		}

		protected Vector2 ScreenToNormalizedPos(Vector2 sp, Camera cam, Vector2 worldOffset)
		{
			return LocalToNormalizedPos(WorldToLocalPos(ScreenToWorldPos(sp, cam), worldOffset));
		}

		protected Vector2 ScreenToNormalizedPos(Vector2 sp, Camera cam)
		{
			return ScreenToNormalizedPos(sp, cam, Vector2.zero);
		}

		protected Vector2 LocalToNormalizedPos(Vector2 lp)
		{
			Rect localRect = GetLocalRect();
			lp -= localRect.center;
			lp.x /= localRect.width * 0.5f;
			lp.y /= localRect.height * 0.5f;
			return lp;
		}

		protected Vector2 WorldToNormalizedPos(Vector2 wp, Vector2 worldOffset)
		{
			return LocalToNormalizedPos(WorldToLocalPos(wp, worldOffset));
		}

		protected Vector2 WorldToNormalizedPos(Vector2 wp)
		{
			return WorldToNormalizedPos(wp, Vector2.zero);
		}

		protected Vector2 NormalizedToLocalPos(Vector2 np)
		{
			Rect localRect = GetLocalRect();
			np.x *= localRect.width * 0.5f;
			np.y *= localRect.height * 0.5f;
			return np + localRect.center;
		}

		protected Vector2 NormalizedToLocalOffset(Vector2 np)
		{
			Rect localRect = GetLocalRect();
			np.x *= localRect.width * 0.5f;
			np.y *= localRect.height * 0.5f;
			return np;
		}

		protected Vector3 NormalizedToWorldPos(Vector2 np)
		{
			Vector2 vector = NormalizedToLocalPos(np);
			return base.transform.localToWorldMatrix.MultiplyPoint3x4(vector);
		}

		protected Vector2 NormalizedToWorldOffset(Vector2 np)
		{
			Vector2 vector = NormalizedToLocalOffset(np);
			return base.transform.localToWorldMatrix.MultiplyVector(vector);
		}

		protected Vector3 WorldToScreenPos(Vector3 wp, Camera cam)
		{
			if (!(cam != null))
			{
				return wp;
			}
			return cam.WorldToScreenPoint(wp);
		}

		protected Vector2 LocalToScreenPos(Vector2 lp, Camera cam)
		{
			return (cam != null) ? cam.WorldToScreenPoint(base.transform.localToWorldMatrix.MultiplyPoint3x4(lp)) : base.transform.localToWorldMatrix.MultiplyPoint3x4(lp);
		}

		public Vector2 ScreenToOrientedPos(Vector2 sp, Camera cam)
		{
			Quaternion identity = Quaternion.identity;
			return Quaternion.Inverse(base.transform.rotation) * sp;
		}

		public Vector3 GetWorldSpaceCenter()
		{
			Vector2 center = GetLocalRect().center;
			if (center == Vector2.zero)
			{
				return base.transform.position;
			}
			return base.transform.localToWorldMatrix.MultiplyPoint3x4(center);
		}

		public Vector3 GetWorldSpaceSize()
		{
			return GetWorldSpaceAABB().size;
		}

		public Bounds GetWorldSpaceAABB()
		{
			return CFUtils.TransformRectAsBounds(GetLocalRect(), base.transform.localToWorldMatrix, shape == Shape.Circle || shape == Shape.Ellipse);
		}

		public Vector2 GetScreenSpaceCenter(Camera cam)
		{
			Vector2 vector = GetWorldSpaceCenter();
			if (!(cam == null))
			{
				return cam.WorldToScreenPoint(vector);
			}
			return vector;
		}

		public Matrix4x4 GetWorldToNormalizedMatrix()
		{
			Rect localRect = GetLocalRect();
			return Matrix4x4.Scale(new Vector3(2f / localRect.width, 2f / localRect.height, 1f)) * Matrix4x4.TRS(-localRect.center, Quaternion.identity, Vector3.one) * base.transform.worldToLocalMatrix;
		}

		public Matrix4x4 GetNormalizedToWorldMatrix()
		{
			Rect localRect = GetLocalRect();
			return base.transform.localToWorldMatrix * Matrix4x4.TRS(localRect.center, Quaternion.identity, new Vector3(localRect.width * 0.5f, localRect.height * 0.5f, 1f));
		}

		protected Vector3 GetFollowPos(Vector3 targetWorldPos, Vector2 worldOffset, out bool posWasOutside)
		{
			Vector3 vector = WorldToNormalizedPos(targetWorldPos, worldOffset);
			if (shape == Shape.Circle || shape == Shape.Ellipse)
			{
				if (vector.sqrMagnitude <= 1f)
				{
					posWasOutside = false;
					return targetWorldPos;
				}
				vector = CFUtils.ClampInsideUnitCircle(vector);
			}
			else
			{
				if (vector.x >= -1f && vector.x <= 1f && vector.y >= -1f && vector.y <= 1f)
				{
					posWasOutside = false;
					return targetWorldPos;
				}
				vector = CFUtils.ClampInsideUnitSquare(vector);
			}
			Vector3 vector2 = NormalizedToWorldPos(vector);
			Vector3 worldSpaceCenter = GetWorldSpaceCenter();
			posWasOutside = true;
			return targetWorldPos - (vector2 - worldSpaceCenter);
		}

		protected Vector3 GetFollowPos(Vector3 targetWorldPos, Vector3 worldOffset)
		{
			bool posWasOutside;
			return GetFollowPos(targetWorldPos, worldOffset, out posWasOutside);
		}

		protected Vector3 ClampInsideCanvas(Vector3 targetWorldPos, Canvas limiterCanvas)
		{
			RectTransform rectTransform = null;
			if (limiterCanvas == null || (rectTransform = limiterCanvas.transform as RectTransform) == null)
			{
				return targetWorldPos;
			}
			Rect localRect = GetLocalRect();
			Rect rect = rectTransform.rect;
			Matrix4x4 tr = limiterCanvas.transform.worldToLocalMatrix * CFUtils.ChangeMatrixTranl(base.transform.localToWorldMatrix, targetWorldPos);
			bool flag = shape == Shape.Circle || shape == Shape.Ellipse;
			Vector2 vector = CFUtils.ClampRectInside(CFUtils.TransformRect(localRect, tr, flag), flag, rect, false);
			if (vector == Vector2.zero)
			{
				return targetWorldPos;
			}
			return targetWorldPos + limiterCanvas.transform.localToWorldMatrix.MultiplyVector(vector);
		}

		protected Vector3 ClampInsideOther(Vector3 targetWorldPos, TouchControl limiter)
		{
			Rect localRect = GetLocalRect();
			Rect localRect2 = limiter.GetLocalRect();
			Matrix4x4 tr = limiter.transform.worldToLocalMatrix * CFUtils.ChangeMatrixTranl(base.transform.localToWorldMatrix, targetWorldPos);
			bool flag = shape == Shape.Circle || shape == Shape.Ellipse;
			Vector2 vector = CFUtils.ClampRectInside(limiterIsRound: limiter.shape == Shape.Circle || limiter.shape == Shape.Ellipse, rect: CFUtils.TransformRect(localRect, tr, flag), rectIsRound: flag, limiterRect: localRect2);
			if (vector == Vector2.zero)
			{
				return targetWorldPos;
			}
			return targetWorldPos + limiter.transform.localToWorldMatrix.MultiplyVector(vector);
		}

		public bool RaycastScreen(Vector2 screenPos, Camera cam)
		{
			return RaycastLocal(ScreenToLocalPos(screenPos, cam));
		}

		public bool RaycastLocal(Vector2 localPos)
		{
			Rect localRect = GetLocalRect();
			switch (shape)
			{
			case Shape.Circle:
			{
				float num = localRect.width * 0.5f;
				return (localPos - localRect.center).sqrMagnitude <= num * num;
			}
			case Shape.Ellipse:
			{
				Vector2 vector = localPos - localRect.center;
				vector.x /= localRect.width * 0.5f;
				vector.y /= localRect.height * 0.5f;
				return vector.sqrMagnitude <= 1f;
			}
			case Shape.Rectangle:
			case Shape.Square:
				if (localPos.x >= localRect.x && localPos.x <= localRect.xMax && localPos.y >= localRect.y)
				{
					return localPos.y <= localRect.yMax;
				}
				return false;
			default:
				return false;
			}
		}

		public bool HitTest(Vector2 sp, Camera cam, float fingerRadPx, Hit hit)
		{
			hit.Reset();
			bool flag = ignoreFingerRadius || fingerRadPx < 0.001f;
			Vector2 vector = ScreenToLocalPos(sp, cam);
			Rect localRect = GetLocalRect();
			bool flag2 = false;
			Vector2 vector2 = Vector2.zero;
			Vector2 vector3 = vector - localRect.center;
			switch (shape)
			{
			case Shape.Circle:
			{
				float num = localRect.width * 0.5f;
				if (!(flag2 = vector3.sqrMagnitude <= num * num) && !flag)
				{
					vector2 = vector3.normalized * num;
				}
				break;
			}
			case Shape.Ellipse:
			{
				Vector2 vector4 = vector3;
				vector4.x /= localRect.width * 0.5f;
				vector4.y /= localRect.height * 0.5f;
				if (!(flag2 = vector4.sqrMagnitude <= 1f) && !flag && !flag)
				{
					vector2 = vector3.normalized;
					vector2.x *= localRect.width * 0.5f;
					vector2.y *= localRect.height * 0.5f;
				}
				break;
			}
			case Shape.Rectangle:
			case Shape.Square:
				if (!(flag2 = vector.x >= localRect.x && vector.x <= localRect.xMax && vector.y >= localRect.y && vector.y <= localRect.yMax) && !flag)
				{
					vector2 = CFUtils.ClampInsideRect(vector, localRect);
				}
				break;
			}
			if (flag && !flag2)
			{
				return false;
			}
			Vector2 vector5 = LocalToScreenPos(vector2, cam);
			bool flag3 = !flag2 && (sp - vector5).sqrMagnitude <= fingerRadPx * fingerRadPx;
			if (flag2 || flag3)
			{
				hit.c = this;
				hit.indirectHit = flag3;
				hit.depth = base.transform.position.z;
				hit.localPos = vector;
				hit.closestLocalPos = (flag2 ? vector : vector2);
				hit.screenDistSqPx = (sp - LocalToScreenPos(localRect.center, cam)).sqrMagnitude;
				hit.screenPos = sp;
				hit.closestScreenPos = vector5;
				return true;
			}
			return false;
		}

		protected override void OnInitComponent()
		{
			StartAlphaAnim(1f, 0f);
			InvalidateHierarchy();
			OnInitControl();
			SyncDisablingConditions(true);
		}

		protected override void OnEnableComponent()
		{
			ResetControl();
			SyncDisablingConditions(true);
		}

		protected override void OnDisableComponent()
		{
			SyncDisablingConditions(true);
			ReleaseAllTouches();
			ResetControl();
		}

		protected override void OnDestroyComponent()
		{
			SetRig(null);
			SetTouchControlPanel(null);
			OnDestroyControl();
		}

		public void UpdateControl()
		{
			if (CanBeUsed())
			{
				UpdateBaseAlpha();
				OnUpdateControl();
				UpdateAnimators(false);
			}
		}

		protected void DrawDefaultGizmo(bool drawFullRect)
		{
			DrawDefaultGizmo(drawFullRect, 0.33f);
		}

		protected void DrawDefaultGizmo(bool drawFullRect, float fullRectColorShade)
		{
		}

		protected virtual void DrawCustomGizmos(bool selected)
		{
			Color color = Gizmos.color;
			Gizmos.color = (selected ? Color.red : Color.white);
			DrawDefaultGizmo(true);
			Gizmos.color = color;
		}

		private void OnDrawGizmos()
		{
			if (base.IsInitialized)
			{
				DrawCustomGizmos(false);
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (base.IsInitialized)
			{
				DrawCustomGizmos(true);
			}
		}
	}
}
