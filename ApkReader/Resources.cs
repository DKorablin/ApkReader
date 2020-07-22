using System.Resources;

namespace AlphaOmega.Debug
{
	internal static class Resources
	{
		private static ResourceManager _permission;
		private static ResourceManager _features;
		private static ResourceManager _intent;

		public static ResourceManager Permission
		{
			get
			{
				return _permission==null
					? _permission = new ResourceManager("AlphaOmega.Debug.Permission", typeof(Resources).Assembly)
					: _permission;
			}
		}

		public static ResourceManager Features
		{
			get
			{
				return _features == null
					? _features = new ResourceManager("AlphaOmega.Debug.Features", typeof(Resources).Assembly)
					: _features;
			}
		}

		public static ResourceManager Intent
		{
			get
			{
				return _intent == null
					? _intent = new System.Resources.ResourceManager("AlphaOmega.Debug.Intent", typeof(Resources).Assembly)
					: _intent;
			}
		}
	}
}