using System;
using UnityEngine;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class TouchGestureThresholds
	{
		public float tapMoveThreshCm;

		public float tapPosThreshCm;

		public float dragThreshCm;

		public float scrollThreshCm;

		public float scrollMagnetFactor;

		public float swipeSegLenCm;

		public float swipeJoystickRadCm;

		public float tapMaxDur;

		public float multiTapMaxTimeGap;

		public float longPressMinTime;

		public float longTapMaxDuration;

		private float _tapMoveThreshPx;

		private float _tapPosThreshPx;

		private float _dragThreshPx;

		private float _swipeSegLenPx;

		private float _swipeJoystickRadPx;

		private float _scrollThreshPx;

		private float storedDpi;

		public float tapMoveThreshPx
		{
			get
			{
				return _tapMoveThreshPx;
			}
		}

		public float tapMoveThreshPxSq
		{
			get
			{
				return _tapMoveThreshPx * _tapMoveThreshPx;
			}
		}

		public float tapPosThreshPx
		{
			get
			{
				return _tapPosThreshPx;
			}
		}

		public float tapPosThreshPxSq
		{
			get
			{
				return _tapPosThreshPx * _tapPosThreshPx;
			}
		}

		public float dragThreshPx
		{
			get
			{
				return _dragThreshPx;
			}
		}

		public float dragThreshPxSq
		{
			get
			{
				return _dragThreshPx * _dragThreshPx;
			}
		}

		public float swipeSegLenPx
		{
			get
			{
				return _swipeSegLenPx;
			}
		}

		public float swipeSegLenPxSq
		{
			get
			{
				return _swipeSegLenPx * _swipeSegLenPx;
			}
		}

		public float swipeJoystickRadPx
		{
			get
			{
				return _swipeJoystickRadPx;
			}
		}

		public float swipeJoystickRadPxSq
		{
			get
			{
				return _swipeJoystickRadPx * _swipeJoystickRadPx;
			}
		}

		public float scrollThreshPx
		{
			get
			{
				return _scrollThreshPx;
			}
		}

		public float scrollThreshPxSq
		{
			get
			{
				return _scrollThreshPx * _scrollThreshPx;
			}
		}

		public TouchGestureThresholds()
		{
			storedDpi = -1f;
			tapMoveThreshCm = 0.1f;
			_tapMoveThreshPx = 10f;
			tapPosThreshCm = 0.5f;
			_tapPosThreshPx = 30f;
			dragThreshCm = 0.25f;
			_dragThreshPx = 10f;
			swipeSegLenCm = 0.5f;
			_swipeSegLenPx = 10f;
			swipeJoystickRadCm = 1.5f;
			_swipeJoystickRadPx = 15f;
			scrollMagnetFactor = 0.1f;
			scrollThreshCm = 0.5f;
			_scrollThreshPx = 30f;
			tapMaxDur = 0.3f;
			multiTapMaxTimeGap = 0.4f;
			longPressMinTime = 0.5f;
			longTapMaxDuration = 1f;
		}

		public void Recalc(float dpi)
		{
			if (dpi <= 0.0001f)
			{
				dpi = 96f;
			}
			if (storedDpi != dpi)
			{
				storedDpi = dpi;
				OnRecalc(dpi / 2.54f);
			}
		}

		protected virtual void OnRecalc(float dpcm)
		{
			_tapMoveThreshPx = Mathf.Max(2f, tapMoveThreshCm * dpcm);
			_tapPosThreshPx = Mathf.Max(2f, tapPosThreshCm * dpcm);
			_dragThreshPx = Mathf.Max(2f, dragThreshCm * dpcm);
			_swipeSegLenPx = Mathf.Max(2f, swipeSegLenCm * dpcm);
			_swipeJoystickRadPx = Mathf.Max(2f, swipeJoystickRadCm * dpcm);
			_scrollThreshPx = Mathf.Max(2f, scrollThreshCm * dpcm);
		}
	}
}
