using BzKovSoft.SmoothMeshConverter.Handlers;
using System;
using System.Collections;
using UnityEngine;

namespace BzKovSoft.SmoothMeshConverter.Samples
{
	public class ConvetModelSample : MonoBehaviour
	{
		public GameObject[] _convertStack;
		public Texture _substitudeTexture;
		public Texture _substitudeNormalMap;
		bool _inProgress;

		private void Start()
		{
			if (_convertStack.Length < 2)
			{
				throw new InvalidOperationException("You forget to add model GameObjects to Convert Stack. There must be at least two models");
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				if (_inProgress)
				{
					return;
				}

				_inProgress = true;
				Convert(_convertStack[0], _convertStack[1], _substitudeTexture, _substitudeNormalMap);
				var tmp = _convertStack[0];
				for (int i = 1; i < _convertStack.Length; i++)
				{
					_convertStack[i - 1] = _convertStack[i];
				}
				_convertStack[_convertStack.Length - 1] = tmp;
			}
		}

		private void Convert(GameObject from, GameObject to, Texture texture, Texture normalMap)
		{
			SetActive(to, true);
			var converter = SMConverter.Blend(from, to);
			converter.BlendTime = 2f;

			converter.AddHandler(new SmcCallbackHandler(OnFinished));
			converter.AddHandler(new SmcZeroPressHandler());
			var texHandler = new SmcBumpDiffMaterialHandler(texture, normalMap);
			converter.AddHandler(texHandler);
		}

		private void SetActive(GameObject go, bool isActive)
		{
			go.SetActive(isActive);

			if (isActive)
			{
				var animator = go.GetComponentInChildren<Animator>();

				// we need to skip one frame. I do not why but unity is ignoring position modification of sub-objects if I just set object to be active
				animator.enabled = false;
				StartCoroutine(EnableAnimatorNextFrame(animator));
			}
		}

		private static IEnumerator EnableAnimatorNextFrame(Animator animator)
		{
			yield return null;
			animator.enabled = true;
		}

		private void OnFinished(SMConverter obj)
		{
			_inProgress = false;
			SetActive(_convertStack[_convertStack.Length - 1], false);
		}
	}
}