namespace Bearshirt
{
	public class GlobalState
	{
		private static bool _IsAtDoor = false;
		public static int Levels = 0;

		private static bool _IsInLava = false;
		public static int Deaths = 0;

		public static bool IsAtDoor
		{
			get { return _IsAtDoor; }
			set
			{
				_IsAtDoor = value;
				if (_IsAtDoor)
				{
					++Levels;
				}
			}
		}

		public static bool IsInLava
		{
			get { return _IsInLava; }
			set
			{
				_IsInLava = value;
				if (_IsInLava)
				{
					++Deaths;
				}
			}
		}
	}
}
