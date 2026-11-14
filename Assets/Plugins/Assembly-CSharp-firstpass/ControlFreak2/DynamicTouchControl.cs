using ControlFreak2.Internal;
using UnityEngine;

namespace ControlFreak2
{
	public abstract class DynamicTouchControl : TouchControl
	{
		public bool fadeOutWhenReleased = true;

		public bool startFadedOut;

		public float fadeOutTargetAlpha;

		public float fadeInDuration = 0.2f;

		public float fadeOutDelay = 0.5f;

		public float fadeOutDuration = 0.5f;

		public bool centerOnDirectTouch = true;

		public bool centerOnIndirectTouch = true;

		public bool centerWhenFollowing;

		public bool stickyMode;

		public bool clampInsideRegion = true;

		public bool clampInsideCanvas = true;

		public bool returnToStartingPosition;

		public Vector2 directInitialVector;

		public Vector2 indirectInitialVector;

		public float touchSmoothing;

		[Tooltip("Stick's origin smoothing - the higher the value, the slower the movement.")]
		[Range(0f, 1f)]
		public float originSmoothTime = 0.5f;

		private const float ORIGIN_ANIM_MAX_TIME = 0.2f;

		public DynamicRegion targetDynamicRegion;

		protected bool linkedToDynamicRegion;

		protected bool touchStartedByRegion;

		protected TouchStartType touchStartType;

		protected TouchGestureBasicState touchStateOriented;

		protected TouchGestureBasicState touchStateScreen;

		protected TouchGestureBasicState touchStateWorld;

		protected TouchObject touchObj;

		private RectTransform initialRectCopy;

		private Vector3 originPos;

		private Vector3 originStartPos;

		private bool originAnimOn;

		private float originAnimElapsed;

		private bool dynamicIsFadingOut;

		private bool dynamicAlphaAnimOn;

		private bool dynamicWaitingToFadeOut;

		private float dynamicAlphaAnimDur;

		private float dynamicAlphaAnimElapsed;

		private float dynamicFadeOutDelayElapsed;

		private float dynamicAlphaCur;

		private float dynamicAlphaStart;

		private float dynamicAlphaTarget;

		private GameObject initialRectCopyGo;

		private Vector2 initialAnchorMax;

		private Vector2 initialAnchorMin;

		private Vector2 initialOffsetMin;

		private Vector2 initialOffsetMax;

		private Vector2 initialPivot;

		private Vector3 initialAnchoredPosition3D;

		public TouchGestureBasicState TouchStateScreen
		{
			get
			{
				return touchStateScreen;
			}
		}

		public TouchGestureBasicState TouchStateOriented
		{
			get
			{
				return touchStateOriented;
			}
		}

		public TouchGestureBasicState TouchStateWorld
		{
			get
			{
				return touchStateWorld;
			}
		}

		public DynamicTouchControl()
		{
			touchStateScreen = new TouchGestureBasicState();
			touchStateWorld = new TouchGestureBasicState();
			touchStateOriented = new TouchGestureBasicState();
			directInitialVector = Vector2.zero;
			indirectInitialVector = Vector2.zero;
			startFadedOut = true;
			fadeOutWhenReleased = true;
			touchSmoothing = 0.1f;
		}

		protected override void OnInitControl()
		{
			SetTargetDynamicRegion(targetDynamicRegion);
			SetTouchSmoothing(touchSmoothing);
			StoreDefaultPos();
		}

		public override void ResetControl()
		{
			if (CanFadeOut() && startFadedOut && !CFUtils.editorStopped)
			{
				DynamicFadeOut(false);
			}
			else
			{
				DynamicWakeUp(false);
			}
		}

		public override void InvalidateHierarchy()
		{
			base.InvalidateHierarchy();
			StoreDefaultPos();
		}

		public bool Pressed()
		{
			return touchStateWorld.PressedRaw();
		}

		public bool JustPressed()
		{
			return touchStateWorld.JustPressedRaw();
		}

		public bool JustReleased()
		{
			return touchStateWorld.JustReleasedRaw();
		}

		public bool IsTouchPressureSensitive()
		{
			if (touchStateWorld.PressedRaw())
			{
				return touchStateWorld.IsPressureSensitive();
			}
			return false;
		}

		public float GetTouchPressure()
		{
			if (!touchStateWorld.PressedRaw())
			{
				return 0f;
			}
			return touchStateWorld.GetPressure();
		}

		public void SetTouchSmoothing(float smTime)
		{
			touchSmoothing = Mathf.Clamp01(smTime);
			touchStateWorld.SetSmoothingTime(touchSmoothing * 0.1f);
			touchStateOriented.SetSmoothingTime(touchSmoothing * 0.1f);
			touchStateScreen.SetSmoothingTime(touchSmoothing * 0.1f);
		}

		public void SetTargetDynamicRegion(DynamicRegion targetDynamicRegion)
		{
			this.targetDynamicRegion = targetDynamicRegion;
			if (targetDynamicRegion != null && targetDynamicRegion.CanBeUsed())
			{
				targetDynamicRegion.SetTargetControl(this);
			}
		}

		public void OnLinkToDynamicRegion(DynamicRegion dynRegion)
		{
			linkedToDynamicRegion = dynRegion != null && dynRegion == targetDynamicRegion;
		}

		public DynamicRegion GetDynamicRegion()
		{
			if (!linkedToDynamicRegion)
			{
				return null;
			}
			return targetDynamicRegion;
		}

		public bool IsInDynamicMode()
		{
			return GetDynamicRegion() != null;
		}

		public float GetDynamicAlpha()
		{
			return dynamicAlphaCur;
		}

		public override float GetAlpha()
		{
			return GetDynamicAlpha() * base.GetAlpha();
		}

		private void SetDynamicAlpha(float alpha, float animDur)
		{
			if (animDur > 0.001f)
			{
				dynamicAlphaAnimDur = animDur;
				dynamicAlphaStart = dynamicAlphaCur;
				dynamicAlphaTarget = alpha;
				dynamicAlphaAnimElapsed = 0f;
				dynamicAlphaAnimOn = true;
			}
			else
			{
				dynamicAlphaAnimOn = false;
				dynamicAlphaAnimElapsed = 0f;
				dynamicAlphaCur = (dynamicAlphaTarget = (dynamicAlphaStart = alpha));
			}
		}

		private void DynamicWakeUp(bool animate)
		{
			dynamicIsFadingOut = false;
			dynamicWaitingToFadeOut = false;
			SetDynamicAlpha(1f, animate ? fadeInDuration : 0f);
		}

		private void DynamicFadeOut(bool animate)
		{
			if (!animate)
			{
				dynamicIsFadingOut = true;
				dynamicWaitingToFadeOut = false;
				dynamicFadeOutDelayElapsed = 0f;
				SetDynamicAlpha(fadeOutTargetAlpha, 0f);
			}
			else if (!dynamicIsFadingOut)
			{
				dynamicIsFadingOut = true;
				dynamicWaitingToFadeOut = true;
				dynamicFadeOutDelayElapsed = 0f;
			}
		}

		private bool CanFadeOut()
		{
			if (fadeOutWhenReleased)
			{
				return IsInDynamicMode();
			}
			return false;
		}

		private void UpdateDynamicAlpha()
		{
			if (dynamicAlphaAnimOn)
			{
				dynamicAlphaAnimElapsed += CFUtils.realDeltaTime;
				if (dynamicAlphaAnimElapsed > dynamicAlphaAnimDur)
				{
					dynamicAlphaAnimOn = false;
					dynamicAlphaCur = dynamicAlphaTarget;
				}
				else
				{
					dynamicAlphaCur = Mathf.Lerp(dynamicAlphaStart, dynamicAlphaTarget, dynamicAlphaAnimElapsed / dynamicAlphaAnimDur);
				}
			}
			if (dynamicIsFadingOut && dynamicWaitingToFadeOut)
			{
				dynamicFadeOutDelayElapsed += CFUtils.realDeltaTime;
				if (dynamicFadeOutDelayElapsed >= fadeOutDelay)
				{
					dynamicWaitingToFadeOut = false;
					SetDynamicAlpha(fadeOutTargetAlpha, fadeOutDuration);
				}
			}
		}

		public override void SetWorldPos(Vector2 pos2D)
		{
			SetOriginPos(pos2D, false);
			StoreDefaultPos();
		}

		protected void SetOriginPos(Vector3 pos, bool animate)
		{
			originPos = pos;
			if (animate)
			{
				originStartPos = GetWorldPos();
				originAnimOn = true;
				originAnimElapsed = 0f;
			}
			else
			{
				SetWorldPosRaw(originPos);
				originStartPos = originPos;
				originAnimOn = false;
			}
		}

		protected void SetOriginPos(Vector3 pos)
		{
			SetOriginPos(pos, true);
		}

		protected Vector2 GetOriginOffset()
		{
			return base.transform.position - originPos;
		}

		protected void UpdateOriginAnimation()
		{
			if (originAnimOn)
			{
				originAnimElapsed += CFUtils.realDeltaTime;
				if (originAnimElapsed >= originSmoothTime * 0.2f)
				{
					originAnimOn = false;
					SetWorldPosRaw(originPos);
				}
				else
				{
					SetWorldPosRaw(Vector3.Lerp(originStartPos, originPos, originAnimElapsed / (originSmoothTime * 0.2f)));
				}
			}
		}

		public override void StoreDefaultPos()
		{
			if (!CFUtils.editorStopped)
			{
				if (initialRectCopyGo == null)
				{
					initialRectCopyGo = new GameObject(base.name + "_INITIAL_POS", typeof(InitialPosPlaceholder));
				}
				RectTransform component = GetComponent<RectTransform>();
				initialRectCopyGo.transform.SetParent(component.parent, false);
				initialRectCopy = initialRectCopyGo.GetComponent<RectTransform>();
				initialRectCopyGo.hideFlags = HideFlags.HideAndDontSave;
				initialAnchorMin = component.anchorMin;
				initialAnchorMax = component.anchorMax;
				initialOffsetMin = component.offsetMin;
				initialOffsetMax = component.offsetMax;
				initialAnchoredPosition3D = component.anchoredPosition3D;
				initialPivot = component.pivot;
				SetupInitialRectPosition();
			}
		}

		private void SetupInitialRectPosition()
		{
			if (!(initialRectCopy == null))
			{
				initialRectCopy.anchoredPosition3D = initialAnchoredPosition3D;
				initialRectCopy.anchorMin = initialAnchorMin;
				initialRectCopy.anchorMax = initialAnchorMax;
				initialRectCopy.offsetMin = initialOffsetMin;
				initialRectCopy.offsetMax = initialOffsetMax;
				initialRectCopy.pivot = initialPivot;
			}
		}

		protected Vector3 GetDefaultPos()
		{
			if (!(initialRectCopy == null))
			{
				return initialRectCopy.position;
			}
			return base.transform.position;
		}

		protected override void OnDestroyControl()
		{
			ResetControl();
			if (initialRectCopyGo != null)
			{
				Object.Destroy(initialRectCopyGo);
			}
		}

		protected override void OnUpdateControl()
		{
			if (touchObj != null && base.rig != null)
			{
				base.rig.WakeTouchControlsUp();
			}
			UpdateDynamicAlpha();
			touchStateWorld.Update();
			touchStateScreen.Update();
			touchStateOriented.Update();
			if (touchStateScreen.JustPressedRaw())
			{
				DynamicWakeUp(true);
				if (!IsInDynamicMode())
				{
					SetOriginPos(GetWorldPos(), false);
				}
				else
				{
					bool flag = GetDynamicAlpha() < 0.001f;
					if (!centerOnDirectTouch && !touchStartedByRegion)
					{
						SetOriginPos(GetWorldPos(), false);
					}
					else
					{
						Vector2 vector = touchStateWorld.GetStartPos();
						if (touchStartedByRegion)
						{
							if (indirectInitialVector != Vector2.zero)
							{
								vector -= NormalizedToWorldOffset(indirectInitialVector);
							}
						}
						else if (directInitialVector != Vector2.zero)
						{
							vector -= NormalizedToWorldOffset(directInitialVector);
						}
						if (!flag && !centerOnIndirectTouch)
						{
							vector = GetFollowPos(vector, Vector2.zero);
						}
						if (clampInsideRegion && GetDynamicRegion() != null)
						{
							vector = ClampInsideOther(vector, GetDynamicRegion());
						}
						if (clampInsideCanvas && base.canvas != null)
						{
							vector = ClampInsideCanvas(vector, base.canvas);
						}
						SetOriginPos(vector, !flag);
					}
				}
			}
			if (touchStateWorld.JustReleasedRaw() && (!IsInDynamicMode() || returnToStartingPosition))
			{
				SetOriginPos(GetDefaultPos(), true);
			}
			if (IsInDynamicMode() && fadeOutWhenReleased && !touchStateWorld.PressedRaw())
			{
				DynamicFadeOut(true);
			}
			touchStartedByRegion = false;
			if (touchStateWorld.PressedRaw() && stickyMode && (swipeOffMode == SwipeOffMode.Disabled || (swipeOffMode == SwipeOffMode.OnlyIfSwipedOver && touchStartType != TouchStartType.SwipeOver)))
			{
				bool posWasOutside = true;
				Vector3 targetWorldPos = touchStateWorld.GetCurPosSmooth();
				if (!centerWhenFollowing)
				{
					posWasOutside = false;
					targetWorldPos = GetFollowPos(targetWorldPos, GetOriginOffset(), out posWasOutside);
				}
				if (posWasOutside)
				{
					if (clampInsideRegion && GetDynamicRegion() != null)
					{
						targetWorldPos = ClampInsideOther(targetWorldPos, GetDynamicRegion());
					}
					if (clampInsideCanvas && base.canvas != null)
					{
						targetWorldPos = ClampInsideCanvas(targetWorldPos, base.canvas);
					}
					SetOriginPos(targetWorldPos);
				}
			}
			UpdateOriginAnimation();
		}

		public override bool OnTouchStart(TouchObject touch, TouchControl sender, TouchStartType touchStartType)
		{
			if (touchObj != null)
			{
				return false;
			}
			touchObj = touch;
			touchObj.AddControl(this);
			Vector3 vector = ((touchStartType == TouchStartType.DirectPress) ? touch.screenPosStart : touch.screenPosCur);
			Vector3 vector2 = touch.screenPosCur;
			Vector3 vector3 = ScreenToOrientedPos(vector, touch.cam);
			Vector3 vector4 = ScreenToOrientedPos(vector2, touch.cam);
			Vector3 vector5 = ScreenToWorldPos(vector, touch.cam);
			Vector3 vector6 = ScreenToWorldPos(vector2, touch.cam);
			touchStateWorld.OnTouchStart(vector5, vector6, 0f, touchObj);
			touchStateScreen.OnTouchStart(vector, vector2, 0f, touchObj);
			touchStateOriented.OnTouchStart(vector3, vector4, 0f, touchObj);
			touchStartedByRegion = sender != null && sender != this;
			this.touchStartType = touchStartType;
			return true;
		}

		public override bool OnTouchEnd(TouchObject touch, TouchEndType touchEndType)
		{
			if (touch != touchObj || touchObj == null)
			{
				return false;
			}
			touchObj = null;
			touchStateWorld.OnTouchEnd(touchEndType != TouchEndType.Release);
			touchStateScreen.OnTouchEnd(touchEndType != TouchEndType.Release);
			touchStateOriented.OnTouchEnd(touchEndType != TouchEndType.Release);
			return true;
		}

		public override bool OnTouchMove(TouchObject touch)
		{
			if (touch != touchObj || touchObj == null)
			{
				return false;
			}
			Vector3 vector = touch.screenPosCur;
			Vector3 vector2 = ScreenToWorldPos(touch.screenPosCur, touch.cam);
			Vector3 vector3 = ScreenToOrientedPos(touch.screenPosCur, touch.cam);
			touchStateWorld.OnTouchMove(vector2);
			touchStateScreen.OnTouchMove(vector);
			touchStateOriented.OnTouchMove(vector3);
			CheckSwipeOff(touch, touchStartType);
			return true;
		}

		public override bool OnTouchPressureChange(TouchObject touch)
		{
			if (touch != touchObj || touchObj == null)
			{
				return false;
			}
			touchStateWorld.OnTouchPressureChange(touch.GetPressure());
			touchStateScreen.OnTouchPressureChange(touch.GetPressure());
			touchStateOriented.OnTouchPressureChange(touch.GetPressure());
			return true;
		}

		public override void ReleaseAllTouches()
		{
			if (touchObj != null)
			{
				touchObj.ReleaseControl(this, TouchEndType.Cancel);
				touchObj = null;
			}
			touchStateWorld.OnTouchEnd(true);
			touchStateOriented.OnTouchEnd(true);
			touchStateScreen.OnTouchEnd(true);
		}

		public override bool CanBeTouchedDirectly(TouchObject touchObj)
		{
			if (base.CanBeTouchedDirectly(touchObj))
			{
				return this.touchObj == null;
			}
			return false;
		}

		public override bool CanBeSwipedOverFromNothing(TouchObject touchObj)
		{
			if (CanBeSwipedOverFromNothingDefault(touchObj) && this.touchObj == null)
			{
				return IsActiveAndVisible();
			}
			return false;
		}

		public override bool CanBeSwipedOverFromRestrictedList(TouchObject touchObj)
		{
			if (CanBeSwipedOverFromRestrictedListDefault(touchObj) && this.touchObj == null)
			{
				return IsActiveAndVisible();
			}
			return false;
		}

		public override bool CanSwipeOverOthers(TouchObject touchObj)
		{
			return CanSwipeOverOthersDefault(touchObj, this.touchObj, touchStartType);
		}

		public virtual bool CanBeActivatedByDynamicRegion()
		{
			if (touchObj == null)
			{
				return IsActive();
			}
			return false;
		}

		public override bool CanBeActivatedByOtherControl(TouchControl c, TouchObject touchObj)
		{
			if (base.CanBeActivatedByOtherControl(c, touchObj))
			{
				return this.touchObj == null;
			}
			return false;
		}
	}
}
