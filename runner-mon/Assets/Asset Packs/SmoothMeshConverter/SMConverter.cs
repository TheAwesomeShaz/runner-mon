using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BzKovSoft.SmoothMeshConverter
{
	/// <summary>
	/// Main component for conversion
	/// </summary>
	public class SMConverter : MonoBehaviour
	{
		[Range(0f, 1f)]
		[SerializeField]
		private float _blendRate = 0f;
		private float _blendTime = 2f;

		private List<ISmcHandler> _handlers;

		public SMConverter()
		{
			_handlers = new List<ISmcHandler>();
			Handlers = _handlers.AsReadOnly();
		}

		private bool _blending;
		private GameObject _other;

		public void AddHandler(ISmcHandler handler)
		{
			handler.Setup(this, gameObject, _other);
			_handlers.Add(handler);
		}

		/// <summary>
		/// Readonly handlers collection
		/// </summary>
		public IList<ISmcHandler> Handlers { get; }
		/// <summary>
		/// Time of conversion. In seconds.
		/// </summary>
		public float BlendTime { get => _blendTime; set => _blendTime = value; }
		/// <summary>
		/// Rate of blend. From 0 to 1
		/// </summary>
		public float BlendRate { get => _blendRate; set => _blendRate = value; }

		/// <summary>
		/// Setup and start blend from one object to another
		/// </summary>
		/// <returns>Converter object attached to one of target objects</returns>
		public static SMConverter Blend(GameObject blendFrom, GameObject blendTo)
		{
			var converter = blendFrom.AddComponent<SMConverter>();
			converter.StartTimer(blendTo);
			return converter;
		}

		/// <summary>
		/// Start the conversion loop. Have to be called after setup method.
		/// </summary>
		public void StartTimer(GameObject other)
		{
			_blending = true;
			_other = other;
			StartCoroutine(StartTimerInner());
		}

		private IEnumerator StartTimerInner()
		{
			for (_blendRate = 0f; _blendRate < 1f;)
			{
				float rateDelta = Time.deltaTime / _blendTime;
				if (_blendRate + rateDelta >= 1f)
				{
					rateDelta = 1f - _blendRate;
					_blendRate = 1f;
				}
				else
				{
					_blendRate += rateDelta;
				}

				yield return null;
			}

			foreach (var handler in _handlers)
			{
				handler.Finished();
			}

			Destroy(this);
		}

		private void Update()
		{
			if (_blending)
			{
				foreach (var handler in _handlers)
				{
					handler.Update();
				}
			}
		}

		private void LateUpdate()
		{
			foreach (var handler in _handlers)
			{
				handler.LateUpdate();
			}
		}
	}
}
