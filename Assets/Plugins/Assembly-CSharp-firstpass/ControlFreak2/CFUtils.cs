using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ControlFreak2
{
	public static class CFUtils
	{
		public const float SqrtOf2 = 1.4142135f;

		public const float SqrtOf3 = 1.7320508f;

		public const float OneOverSqrtOf2 = 0.70710677f;

		public const float OneOverSqrtOf3 = 0.57735026f;

		public const float TanLenFor45Tri = 0.76536685f;

		public const float TanLenFor90Tri = 1.4142135f;

		public const float MAX_DELTA_TIME = 0.5f;

		private const float MAX_LEFT_FACTOR = 0.75f;

		public static float realDeltaTime
		{
			get
			{
				if (Time.captureFramerate > 0)
				{
					return 1f / (float)Time.captureFramerate;
				}
				return Time.unscaledDeltaTime;
			}
		}

		public static float realDeltaTimeClamped
		{
			get
			{
				return Mathf.Min(realDeltaTime, 0.5f);
			}
		}

		public static bool editorStopped
		{
			get
			{
				return false;
			}
		}

		public static bool forcedMobileModeEnabled
		{
			get
			{
				return true;
			}
		}

		public static string LogPrefixFull()
		{
			return "[" + Time.frameCount + "] ";
		}

		public static string LogPrefix()
		{
			return "[" + Time.frameCount + "] ";
		}

		public static float ApplyDeltaInput(float accum, float delta)
		{
			if (accum >= 0f && delta >= 0f)
			{
				return Mathf.Max(accum, delta);
			}
			if (accum < 0f && delta < 0f)
			{
				return Mathf.Min(accum, delta);
			}
			return accum + delta;
		}

		public static int ApplyDeltaInputInt(int accum, int delta)
		{
			if (accum >= 0 && delta >= 0)
			{
				return Mathf.Max(accum, delta);
			}
			if (accum < 0 && delta < 0)
			{
				return Mathf.Min(accum, delta);
			}
			return accum + delta;
		}

		public static float ApplyPositveDeltaInput(float positiveAccum, float delta)
		{
			if (!(delta > 0f) || !(delta > positiveAccum))
			{
				return positiveAccum;
			}
			return delta;
		}

		public static float ApplyNegativeDeltaInput(float negativeAccum, float delta)
		{
			if (!(delta < 0f) || !(delta < negativeAccum))
			{
				return negativeAccum;
			}
			return delta;
		}

		public static void ApplySignedDeltaInput(float v, ref float plusAccum, ref float minusAccum)
		{
			if (v >= 0f)
			{
				if (v > plusAccum)
				{
					plusAccum = v;
				}
			}
			else if (v < minusAccum)
			{
				minusAccum = v;
			}
		}

		public static void ApplySignedDeltaInputInt(int v, ref int plusAccum, ref int minusAccum)
		{
			if (v >= 0)
			{
				if (v > plusAccum)
				{
					plusAccum = v;
				}
			}
			else if (v < minusAccum)
			{
				minusAccum = v;
			}
		}

		public static int GetScrollValue(float drag, int prevScroll, float thresh, float magnet)
		{
			bool num = drag < 0f;
			if (num)
			{
				drag = 0f - drag;
				prevScroll = -prevScroll;
			}
			int num2 = Mathf.FloorToInt(drag / thresh);
			if (!num)
			{
				return num2;
			}
			return -num2;
		}

		public static float MoveTowards(float a, float b, float secondsPerUnit, float deltaTime, float epsilon)
		{
			if (Mathf.Abs(b - a) <= epsilon)
			{
				return b;
			}
			if (!(secondsPerUnit < 0.001f) && !(secondsPerUnit <= deltaTime))
			{
				return Mathf.MoveTowards(a, b, deltaTime / secondsPerUnit);
			}
			return b;
		}

		private static float GetLerpFactor(float smoothingTime, float deltaTime, float maxLerpFactor)
		{
			return Mathf.Min(maxLerpFactor, (smoothingTime <= deltaTime) ? 1f : (deltaTime / smoothingTime));
		}

		public static float SmoothTowards(float a, float b, float smoothingTime, float deltaTime, float epsilon, float maxLeftFactor = 0.75f)
		{
			if (Mathf.Abs(b - a) <= epsilon)
			{
				return b;
			}
			if (!(smoothingTime < 0.001f))
			{
				return Mathf.Lerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor));
			}
			return b;
		}

		public static float SmoothTowardsAngle(float a, float b, float smoothingTime, float deltaTime, float epsilon, float maxLeftFactor = 0.75f)
		{
			if (Mathf.Abs(Mathf.DeltaAngle(b, a)) <= epsilon)
			{
				return b;
			}
			if (!(smoothingTime < 0.001f))
			{
				return Mathf.MoveTowardsAngle(a, b, Mathf.Abs(b - a) * GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor));
			}
			return b;
		}

		public static Vector2 SmoothTowardsVec2(Vector2 a, Vector2 b, float smoothingTime, float deltaTime, float sqrEpsilon, float maxLeftFactor = 0.75f)
		{
			if (sqrEpsilon != 0f && (b - a).sqrMagnitude <= sqrEpsilon)
			{
				return b;
			}
			if (!(smoothingTime < 0.001f))
			{
				return Vector2.Lerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor));
			}
			return b;
		}

		public static Vector3 SmoothTowardsVec3(Vector3 a, Vector3 b, float smoothingTime, float deltaTime, float sqrEpsilon, float maxLeftFactor = 0.75f)
		{
			if (sqrEpsilon != 0f && (b - a).sqrMagnitude <= sqrEpsilon)
			{
				return b;
			}
			if (!(smoothingTime < 0.001f))
			{
				return Vector3.Lerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor));
			}
			return b;
		}

		public static Color SmoothTowardsColor(Color a, Color b, float smoothingTime, float deltaTime, float maxLeftFactor = 0.75f)
		{
			if (!(smoothingTime < 0.001f))
			{
				return Color.Lerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor));
			}
			return b;
		}

		public static Quaternion SmoothTowardsQuat(Quaternion a, Quaternion b, float smoothingTime, float deltaTime, float maxLeftFactor = 0.75f)
		{
			if (!(smoothingTime < 0.001f))
			{
				return Quaternion.Slerp(a, b, GetLerpFactor(smoothingTime, deltaTime, maxLeftFactor));
			}
			return b;
		}

		public static float SmoothDamp(float valFrom, float valTo, ref float vel, float smoothingTime, float deltaTime, float epsilon)
		{
			if (smoothingTime < 0.001f || Mathf.Abs(valTo - valFrom) <= epsilon)
			{
				vel = 0f;
				return valTo;
			}
			return Mathf.SmoothDamp(valFrom, valTo, ref vel, smoothingTime, 10000000f, deltaTime);
		}

		public static Color ScaleColorAlpha(Color color, float alphaScale)
		{
			color.a *= alphaScale;
			return color;
		}

		public static Component GetComponentHereOrInParent(Component comp, Type compType)
		{
			if (comp == null)
			{
				return null;
			}
			Component component = null;
			if (!((component = comp.GetComponent(compType)) != null))
			{
				return comp.GetComponentInParent(compType);
			}
			return component;
		}

		public static bool IsStretchyRectTransform(Transform tr)
		{
			RectTransform rectTransform = tr as RectTransform;
			if (rectTransform != null)
			{
				return rectTransform.anchorMax != rectTransform.anchorMin;
			}
			return false;
		}

		public static Rect TransformRect(Rect r, Matrix4x4 tr, bool round)
		{
			Bounds bounds = TransformRectAsBounds(r, tr, round);
			Vector3 min = bounds.min;
			Vector3 size = bounds.size;
			return new Rect(min.x, min.y, size.x, size.y);
		}

		public static Bounds TransformRectAsBounds(Rect r, Matrix4x4 tr, bool round)
		{
			Vector3 vector = tr.MultiplyPoint3x4(r.center);
			Vector3 vector2 = tr.MultiplyVector(new Vector3(1f, 0f, 0f));
			Vector3 vector3 = tr.MultiplyVector(new Vector3(0f, 1f, 0f));
			float num = r.width * 0.5f;
			float num2 = r.height * 0.5f;
			vector2 *= num;
			vector3 *= num2;
			Vector3 lhs;
			Vector3 lhs2;
			if (round)
			{
				Vector3 vector4 = vector3;
				lhs = Vector3.Min(vector4, -vector4);
				lhs2 = Vector3.Max(vector4, -vector4);
				vector4 = vector2 * 0.77f + vector3 * 0.77f;
				lhs = Vector3.Min(lhs, Vector3.Min(vector4, -vector4));
				lhs2 = Vector3.Max(lhs2, Vector3.Max(vector4, -vector4));
				vector4 = vector2;
				lhs = Vector3.Min(lhs, Vector3.Min(vector4, -vector4));
				lhs2 = Vector3.Max(lhs2, Vector3.Max(vector4, -vector4));
				vector4 = vector2 * -0.77f + vector3 * 0.77f;
				lhs = Vector3.Min(lhs, Vector3.Min(vector4, -vector4));
				lhs2 = Vector3.Max(lhs2, Vector3.Max(vector4, -vector4));
			}
			else
			{
				Vector3 vector4 = vector2 + vector3;
				lhs = Vector3.Min(vector4, -vector4);
				lhs2 = Vector3.Max(vector4, -vector4);
				vector4 = vector2 - vector3;
				lhs = Vector3.Min(lhs, Vector3.Min(vector4, -vector4));
				lhs2 = Vector3.Max(lhs2, Vector3.Max(vector4, -vector4));
			}
			return new Bounds(vector + (lhs + lhs2) * 0.5f, lhs2 - lhs);
		}

		public static Matrix4x4 ChangeMatrixTranl(Matrix4x4 m, Vector3 newTransl)
		{
			m.SetColumn(3, new Vector4(newTransl.x, newTransl.y, newTransl.z, m.m33));
			return m;
		}

		public static Bounds GetWorldAABB(Matrix4x4 tf, Vector3 center, Vector3 size)
		{
			Vector3 vector = center - size * 0.5f;
			Vector3 vector2 = center + size * 0.5f;
			Vector3 vector3 = tf.MultiplyPoint3x4(new Vector3(vector.x, vector.y, vector.z));
			Vector3 lhs2;
			Vector3 lhs = (lhs2 = vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector.x, vector.y, vector2.z));
			lhs = Vector3.Min(lhs, vector3);
			lhs2 = Vector3.Max(lhs2, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector.x, vector2.y, vector.z));
			lhs = Vector3.Min(lhs, vector3);
			lhs2 = Vector3.Max(lhs2, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector.x, vector2.y, vector2.z));
			lhs = Vector3.Min(lhs, vector3);
			lhs2 = Vector3.Max(lhs2, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector2.x, vector.y, vector.z));
			lhs = Vector3.Min(lhs, vector3);
			lhs2 = Vector3.Max(lhs2, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector2.x, vector2.y, vector.z));
			lhs = Vector3.Min(lhs, vector3);
			lhs2 = Vector3.Max(lhs2, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector2.x, vector.y, vector2.z));
			lhs = Vector3.Min(lhs, vector3);
			lhs2 = Vector3.Max(lhs2, vector3);
			vector3 = tf.MultiplyPoint3x4(new Vector3(vector2.x, vector2.y, vector2.z));
			lhs = Vector3.Min(lhs, vector3);
			lhs2 = Vector3.Max(lhs2, vector3);
			return new Bounds((lhs + lhs2) * 0.5f, lhs2 - lhs);
		}

		public static Bounds GetWorldAABB(Matrix4x4 tf, Bounds localBounds)
		{
			return GetWorldAABB(tf, localBounds.center, localBounds.size);
		}

		public static Rect GetWorldRect(Matrix4x4 tf, Bounds localBounds)
		{
			Bounds worldAABB = GetWorldAABB(tf, localBounds);
			Vector3 min = worldAABB.min;
			Vector3 max = worldAABB.max;
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}

		public static Vector2 ClampRectInside(Rect rect, bool rectIsRound, Rect limiterRect, bool limiterIsRound)
		{
			if (limiterIsRound)
			{
				if (rectIsRound)
				{
					return ClampEllipseInsideEllipse(rect, limiterRect);
				}
				return ClampRectInsideEllipse(rect, limiterRect);
			}
			return ClampRectInsideRect(rect, limiterRect);
		}

		public static Vector2 ClampRectInsideEllipse(Rect rect, Rect limiterRect)
		{
			return ClampEllipseInsideEllipse(rect, limiterRect);
		}

		public static Vector2 ClampEllipseInsideEllipse(Rect rect, Rect limiterRect)
		{
			Vector2 vector = rect.size * 0.5f;
			Vector2 vector2 = limiterRect.size * 0.5f;
			Vector2 center = rect.center;
			Vector2 center2 = limiterRect.center;
			if (vector.x >= vector2.x || vector.y >= vector2.y)
			{
				Vector2 result = default(Vector2);
				if (vector.x >= vector2.x)
				{
					result.x = center2.x - center.x;
				}
				else
				{
					result.x = ((center.x >= center2.x) ? Mathf.Min(0f, center2.x + vector2.x - (center.x + vector.x)) : Mathf.Max(0f, center2.x - vector2.x - (center.x - vector.x)));
				}
				if (vector.y >= vector2.y)
				{
					result.y = center2.y - center.y;
				}
				else
				{
					result.y = ((center.y >= center2.y) ? Mathf.Min(0f, center2.y + vector2.y - (center.y + vector.y)) : Mathf.Max(0f, center2.y - vector2.y - (center.y - vector.y)));
				}
				return result;
			}
			Vector2 vector3 = vector2 - vector;
			Vector2 vector4 = center - center2;
			Vector2 vector5 = vector4;
			vector5.x /= vector3.x;
			vector5.y /= vector3.y;
			if (vector5.sqrMagnitude < 1f)
			{
				return Vector2.zero;
			}
			vector5.Normalize();
			vector5.x *= vector3.x;
			vector5.y *= vector3.y;
			return vector5 - vector4;
		}

		public static Vector2 ClampRectInsideRect(Rect rect, Rect limiterRect)
		{
			Vector2 center = rect.center;
			Vector2 center2 = limiterRect.center;
			Vector2 vector = rect.size * 0.5f;
			Vector2 vector2 = limiterRect.size * 0.5f;
			Vector2 vector3 = center - vector;
			Vector2 vector4 = center + vector;
			Vector2 vector5 = center2 - vector2;
			Vector2 vector6 = center2 + vector2;
			Vector2 zero = Vector2.zero;
			if (vector.x >= vector2.x)
			{
				zero.x = center2.x - center.x;
			}
			else if (vector4.x > vector6.x)
			{
				zero.x = vector6.x - vector4.x;
			}
			else if (vector3.x < vector5.x)
			{
				zero.x = vector5.x - vector3.x;
			}
			if (vector.y >= vector2.y)
			{
				zero.y = center2.y - center.y;
			}
			else if (vector4.y > vector6.y)
			{
				zero.y = vector6.y - vector4.y;
			}
			else if (vector3.y < vector5.y)
			{
				zero.y = vector5.y - vector3.y;
			}
			return zero;
		}

		public static Vector2 ClampInsideUnitCircle(Vector2 np)
		{
			if (!(np.sqrMagnitude < 1f))
			{
				return np.normalized;
			}
			return np;
		}

		public static Vector2 ClampInsideUnitSquare(Vector2 np)
		{
			float num = 1f;
			if (np.x > 1f || np.x < -1f)
			{
				num = Mathf.Abs(1f / np.x);
			}
			if (np.y > 1f || np.y < -1f)
			{
				num = Mathf.Min(num, Mathf.Abs(1f / np.y));
			}
			if (!(num < 1f))
			{
				return np;
			}
			return np * num;
		}

		public static Vector2 ClampPerAxisInsideUnitSquare(Vector2 np)
		{
			np.x = Mathf.Clamp(np.x, -1f, 1f);
			np.y = Mathf.Clamp(np.y, -1f, 1f);
			return np;
		}

		public static Vector2 ClampInsideRect(Vector2 v, Rect r)
		{
			if (r.Contains(v))
			{
				return v;
			}
			Vector2 center = r.center;
			Vector2 vector = r.size * 0.5f;
			v -= center;
			float num = 1f;
			if (v.x > vector.x || v.x < 0f - vector.x)
			{
				num = Mathf.Abs(vector.x / v.x);
			}
			if (v.y > vector.y || v.y < 0f - vector.y)
			{
				num = Mathf.Min(num, Mathf.Abs(vector.y / v.y));
			}
			return center + v * num;
		}

		public static bool IsDirDiagonal(Dir dir)
		{
			return ((dir - 1) & Dir.U) == Dir.U;
		}

		public static float VecToAngle(Vector2 vec)
		{
			return NormalizeAnglePositive(Mathf.Atan2(vec.x, vec.y) * 57.29578f);
		}

		public static float VecToAngle(Vector2 vec, float defaultAngle, float deadZoneSq)
		{
			float sqrMagnitude = vec.sqrMagnitude;
			if (sqrMagnitude < deadZoneSq)
			{
				return defaultAngle;
			}
			if (Mathf.Abs(sqrMagnitude - 1f) > 0.0001f)
			{
				vec.Normalize();
			}
			return NormalizeAnglePositive(Mathf.Atan2(vec.x, vec.y) * 57.29578f);
		}

		public static float DirToAngle(Dir d)
		{
			if (d < Dir.U || d > Dir.UL)
			{
				return 0f;
			}
			return (float)(d - 1) * 45f;
		}

		public static Dir DirFromAngle(float ang, bool as8way)
		{
			ang += (as8way ? 22.5f : 45f);
			ang = NormalizeAnglePositive(ang);
			Dir dir = Dir.N;
			if (as8way)
			{
				if (ang < 45f)
				{
					return Dir.U;
				}
				if (ang < 90f)
				{
					return Dir.UR;
				}
				if (ang < 135f)
				{
					return Dir.R;
				}
				if (ang < 180f)
				{
					return Dir.DR;
				}
				if (ang < 225f)
				{
					return Dir.D;
				}
				if (ang < 270f)
				{
					return Dir.DL;
				}
				if (ang < 315f)
				{
					return Dir.L;
				}
				return Dir.UL;
			}
			if (ang < 90f)
			{
				return Dir.U;
			}
			if (ang < 180f)
			{
				return Dir.R;
			}
			if (ang < 270f)
			{
				return Dir.D;
			}
			return Dir.L;
		}

		public static Dir DirFromAngleEx(float ang, bool as8way, Dir lastDir, float magnetPow)
		{
			if (lastDir != Dir.N && magnetPow > 0.001f && Mathf.Abs(Mathf.DeltaAngle(ang, DirToAngle(lastDir))) < (1f + Mathf.Clamp01(magnetPow) * 0.5f) * (as8way ? 22.5f : 45f))
			{
				return lastDir;
			}
			return DirFromAngle(ang, as8way);
		}

		public static int DirDeltaAngle(Dir dirFrom, Dir dirTo)
		{
			if (dirFrom == Dir.N || dirTo == Dir.N)
			{
				return 0;
			}
			return Mathf.RoundToInt(Mathf.DeltaAngle(DirToAngle(dirFrom), DirToAngle(dirTo)));
		}

		public static Vector2 DirToNormal(Dir dir)
		{
			switch (dir)
			{
			case Dir.U:
				return new Vector2(0f, 1f);
			case Dir.R:
				return new Vector2(1f, 0f);
			case Dir.D:
				return new Vector2(0f, -1f);
			case Dir.L:
				return new Vector2(-1f, 0f);
			case Dir.UR:
				return new Vector2(0.70710677f, 0.70710677f);
			case Dir.DR:
				return new Vector2(0.70710677f, -0.70710677f);
			case Dir.DL:
				return new Vector2(-0.70710677f, -0.70710677f);
			case Dir.UL:
				return new Vector2(-0.70710677f, 0.70710677f);
			default:
				return Vector2.zero;
			}
		}

		public static Vector2 DirToTangent(Dir dir)
		{
			switch (dir)
			{
			case Dir.U:
				return new Vector2(1f, 0f);
			case Dir.R:
				return new Vector2(0f, -1f);
			case Dir.D:
				return new Vector2(-1f, 0f);
			case Dir.L:
				return new Vector2(0f, 1f);
			case Dir.UR:
				return new Vector2(0.70710677f, -0.70710677f);
			case Dir.DR:
				return new Vector2(-0.70710677f, -0.70710677f);
			case Dir.DL:
				return new Vector2(-0.70710677f, 0.70710677f);
			case Dir.UL:
				return new Vector2(0.70710677f, 0.70710677f);
			default:
				return Vector2.zero;
			}
		}

		public static Dir GetOppositeDir(Dir dir)
		{
			switch (dir)
			{
			case Dir.U:
				return Dir.D;
			case Dir.UR:
				return Dir.DL;
			case Dir.R:
				return Dir.L;
			case Dir.DR:
				return Dir.UL;
			case Dir.D:
				return Dir.U;
			case Dir.DL:
				return Dir.UR;
			case Dir.L:
				return Dir.R;
			case Dir.UL:
				return Dir.DR;
			default:
				return Dir.N;
			}
		}

		public static Vector2 DirToVector(Dir dir, bool circular)
		{
			switch (dir)
			{
			case Dir.U:
				return new Vector2(0f, 1f);
			case Dir.R:
				return new Vector2(1f, 0f);
			case Dir.D:
				return new Vector2(0f, -1f);
			case Dir.L:
				return new Vector2(-1f, 0f);
			case Dir.UR:
				if (!circular)
				{
					return new Vector2(1f, 1f);
				}
				return new Vector2(0.70710677f, 0.70710677f).normalized;
			case Dir.DR:
				if (!circular)
				{
					return new Vector2(1f, -1f);
				}
				return new Vector2(0.70710677f, -0.70710677f).normalized;
			case Dir.DL:
				if (!circular)
				{
					return new Vector2(-1f, -1f);
				}
				return new Vector2(-0.70710677f, -0.70710677f).normalized;
			case Dir.UL:
				if (!circular)
				{
					return new Vector2(-1f, 1f);
				}
				return new Vector2(-0.70710677f, 0.70710677f).normalized;
			default:
				return Vector2.zero;
			}
		}

		public static Dir VecToDir(Vector2 vec, bool as8way)
		{
			return DirFromAngle(VecToAngle(vec), as8way);
		}

		public static Dir VecToDir(Vector2 vec, Dir defaultDir, float deadZoneSq, bool as8way)
		{
			float sqrMagnitude = vec.sqrMagnitude;
			if (sqrMagnitude <= deadZoneSq)
			{
				return defaultDir;
			}
			if (Mathf.Abs(sqrMagnitude - 1f) > 1E-05f)
			{
				vec.Normalize();
			}
			return DirFromAngle(VecToAngle(vec), as8way);
		}

		public static float NormalizeAnglePositive(float a)
		{
			if (a >= 360f)
			{
				return Mathf.Repeat(a, 360f);
			}
			if (a >= 0f)
			{
				return a;
			}
			if (a <= -360f)
			{
				a = Mathf.Repeat(a, 360f);
			}
			return 360f + a;
		}

		public static float SmartDeltaAngle(float startAngle, float curAngle, float lastDelta)
		{
			float num = Mathf.DeltaAngle(startAngle + lastDelta, curAngle);
			return lastDelta + num;
		}

		public static Dir DigitalToDir(bool digiU, bool digiR, bool digiD, bool digiL)
		{
			if (digiU && digiD)
			{
				digiU = (digiD = false);
			}
			if (digiR && digiL)
			{
				digiR = (digiL = false);
			}
			if (digiU)
			{
				if (!digiR)
				{
					if (!digiL)
					{
						return Dir.U;
					}
					return Dir.UL;
				}
				return Dir.UR;
			}
			if (digiD)
			{
				if (!digiR)
				{
					if (!digiL)
					{
						return Dir.D;
					}
					return Dir.DL;
				}
				return Dir.DR;
			}
			if (!digiR)
			{
				if (!digiL)
				{
					return Dir.N;
				}
				return Dir.L;
			}
			return Dir.R;
		}

		public static Vector2 CircularToSquareJoystickVec(Vector2 circularVec)
		{
			if (circularVec.sqrMagnitude < 1E-05f)
			{
				return Vector2.zero;
			}
			Vector2 result = circularVec;
			Vector2 normalized = circularVec.normalized;
			result *= Mathf.Abs(normalized.x) + Mathf.Abs(normalized.y);
			result.x = Mathf.Clamp(result.x, -1f, 1f);
			result.y = Mathf.Clamp(result.y, -1f, 1f);
			return result;
		}

		public static Vector2 SquareToCircularJoystickVec(Vector2 squareVec)
		{
			if (squareVec.sqrMagnitude < 1E-05f)
			{
				return Vector2.zero;
			}
			Vector2 result = squareVec;
			Vector2 normalized = squareVec.normalized;
			result /= Mathf.Abs(normalized.x) + Mathf.Abs(normalized.y);
			result.x = Mathf.Clamp(result.x, -1f, 1f);
			result.y = Mathf.Clamp(result.y, -1f, 1f);
			return result;
		}

		public static int GetLineNumber(string str, int index)
		{
			int num = 1;
			int startIndex = 0;
			int num2 = -1;
			while ((num2 = str.IndexOf('\n', startIndex)) >= 0 && num2 < index)
			{
				num++;
				startIndex = num2 + 1;
			}
			return num;
		}

		public static int GetEnumMaxValue(Type enumType)
		{
			int num = 0;
			foreach (int value in Enum.GetValues(enumType))
			{
				num = ((num == 0) ? value : Mathf.Max(num, value));
			}
			return num;
		}

		public static int CycleInt(int curId, int dir, int maxId)
		{
			curId += dir;
			if (curId < 0)
			{
				curId = maxId;
			}
			else if (curId > maxId)
			{
				curId = 0;
			}
			return curId;
		}

		public static void SetEventSystemSelectedObject(GameObject o)
		{
			EventSystem current = EventSystem.current;
			if (current != null)
			{
				current.firstSelectedGameObject = o;
				if (current.currentInputModule is GamepadInputModule)
				{
					current.SetSelectedGameObject(null, null);
					current.SetSelectedGameObject(o, null);
				}
			}
		}
	}
}
