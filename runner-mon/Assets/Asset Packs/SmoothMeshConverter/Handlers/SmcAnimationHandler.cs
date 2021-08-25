using UnityEngine;

namespace BzKovSoft.SmoothMeshConverter.Handlers
{
	public class SmcAnimationHandler : ISmcHandler
	{
		public SmcAnimationHandler()
		{
		}

		public void Setup(SMConverter converter, GameObject thisObject, GameObject other)
		{
			var animatorA = thisObject.GetComponent<Animator>();
			var animatorB = other.GetComponent<Animator>();
			var statesA = animatorA.GetCurrentAnimatorStateInfo(0);
			animatorB.Play(statesA.shortNameHash, 0, statesA.normalizedTime);
		}

		public void Update()
		{
		}

		public void LateUpdate()
		{
		}

		public void Finished()
		{
		}
	}
}
