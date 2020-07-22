using System;

namespace AlphaOmega.Debug.Manifest
{
	/// <summary>Parent node for the Intent-filter node</summary>
	public interface IApkIntentedNode
	{
		/// <summary>Label</summary>
		String Label { get; }

		/// <summary>Icon</summary>
		String Icon { get; }
	}
}