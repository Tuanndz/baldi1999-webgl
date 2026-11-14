using System;

namespace ControlFreak2.Internal
{
	[Serializable]
	public class TouchGestureConfig
	{
		public enum DirMode
		{
			FourWay = 0,
			EightWay = 1
		}

		public enum DirConstraint
		{
			None = 0,
			Horizontal = 1,
			Vertical = 2,
			Auto = 3
		}

		public int maxTapCount;

		public bool cleanTapsOnly;

		public bool detectLongPress;

		public bool detectLongTap;

		public bool endLongPressWhenMoved;

		public bool endLongPressWhenSwiped;

		public DirMode dirMode;

		public DirectionState.OriginalDirResetMode swipeOriginalDirResetMode;

		public DirConstraint swipeConstraint;

		public DirConstraint swipeDirConstraint;

		public DirConstraint scrollConstraint;

		public TouchGestureConfig()
		{
			maxTapCount = 1;
			cleanTapsOnly = true;
			swipeOriginalDirResetMode = DirectionState.OriginalDirResetMode.On180;
		}
	}
}
