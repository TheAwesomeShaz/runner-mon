using UnityEngine;

namespace BzKovSoft.SmoothMeshConverter
{
	public interface ISmcHandler
	{
		/// <summary>
		/// Setup method. Executed ones at the start of conversion
		/// </summary>
		/// <param name="converter">Converter component</param>
		/// <param name="thisObject">Object that we are converting from</param>
		/// <param name="other">Object that we are converting to</param>
		void Setup(SMConverter converter, GameObject thisObject, GameObject other);
		/// <summary>
		/// Called each frame from regular Update method
		/// </summary>
		void Update();
		/// <summary>
		/// Called each frame from regular LateUpdate method
		/// </summary>
		void LateUpdate();
		/// <summary>
		/// Finish of conversion
		/// </summary>
		void Finished();
	}
}
