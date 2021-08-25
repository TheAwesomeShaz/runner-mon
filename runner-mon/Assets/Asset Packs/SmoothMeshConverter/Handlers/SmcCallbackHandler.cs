using System;
using UnityEngine;

namespace BzKovSoft.SmoothMeshConverter.Handlers
{
	public class SmcCallbackHandler : ISmcHandler
	{
		Action<SMConverter> _callback;
		SMConverter _converter;

		public SmcCallbackHandler(Action<SMConverter> callback)
		{
			_callback = callback;
		}

		public void Setup(SMConverter converter, GameObject thisObject, GameObject other)
		{
			_converter = converter;
		}

		public void Update()
		{
		}

		public void LateUpdate()
		{
		}

		public void Finished()
		{
			_callback(_converter);
		}
	}
}
