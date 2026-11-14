using UnityEngine;

namespace ControlFreak2.Internal
{
	public class DirectionState
	{
		public enum OriginalDirResetMode
		{
			OnNeutral = 0,
			On180 = 1,
			On135 = 2,
			On90 = 3
		}

		private Dir dirCur;

		private Dir dirPrev;

		private Dir dirOriginalCur;

		private Dir dirOriginalPrev;

		public DirectionState()
		{
			Reset();
		}

		public Dir GetCur()
		{
			return dirCur;
		}

		public Dir GetPrev()
		{
			return dirPrev;
		}

		public Dir GetOriginal()
		{
			return dirOriginalCur;
		}

		public Dir GetPrevOriginal()
		{
			return dirOriginalPrev;
		}

		public bool JustPressed(Dir dir)
		{
			if (dirPrev != dir)
			{
				return dirCur == dir;
			}
			return false;
		}

		public bool JustReleased(Dir dir)
		{
			if (dirPrev == dir)
			{
				return dirCur != dir;
			}
			return false;
		}

		public void Reset()
		{
			dirCur = Dir.N;
			dirPrev = Dir.N;
			dirOriginalCur = Dir.N;
			dirOriginalPrev = Dir.N;
		}

		public void BeginFrame()
		{
			dirPrev = dirCur;
			dirOriginalPrev = dirOriginalCur;
		}

		public void SetDir(Dir dir, OriginalDirResetMode resetMode)
		{
			dirCur = dir;
			if (dirCur == dirPrev)
			{
				return;
			}
			if (dirCur == Dir.N)
			{
				dirOriginalCur = Dir.N;
			}
			else if (dirPrev == Dir.N)
			{
				dirOriginalCur = dirCur;
			}
			else if (resetMode != OriginalDirResetMode.OnNeutral)
			{
				int num = Mathf.Abs(CFUtils.DirDeltaAngle(dirOriginalPrev, dirCur));
				int num2;
				switch (resetMode)
				{
				default:
					num2 = 180;
					break;
				case OriginalDirResetMode.On135:
					num2 = 135;
					break;
				case OriginalDirResetMode.On90:
					num2 = 90;
					break;
				}
				if (num >= num2)
				{
					dirOriginalCur = dirCur;
				}
			}
		}
	}
}
