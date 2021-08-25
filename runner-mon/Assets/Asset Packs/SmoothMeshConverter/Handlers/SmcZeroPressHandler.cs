using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BzKovSoft.SmoothMeshConverter.Handlers
{
	public class SmcZeroPressHandler : ISmcHandler
	{
		public const string ZeroWidthKey = "ZeroWidth";
		private SMConverter _converter;
		private BonesDetails _bonesDet;

		/// <summary>
		/// Root to leave ordered bones. It's impotent for updating
		/// </summary>
		private static HumanBodyBones[] _bones = new HumanBodyBones[]
		{
				HumanBodyBones.Hips,

				// legs
				HumanBodyBones.LeftUpperLeg,
				HumanBodyBones.RightUpperLeg,
				HumanBodyBones.LeftLowerLeg,
				HumanBodyBones.RightLowerLeg,
				HumanBodyBones.LeftFoot,
				HumanBodyBones.RightFoot,
				HumanBodyBones.LeftToes,
				HumanBodyBones.RightToes,

				// spine
				HumanBodyBones.Spine,
				HumanBodyBones.Chest,
				HumanBodyBones.UpperChest,

				// head
				HumanBodyBones.Neck,
				HumanBodyBones.Head,
				HumanBodyBones.LeftEye,
				HumanBodyBones.RightEye,
				HumanBodyBones.Jaw,

				// hands
				HumanBodyBones.LeftShoulder,
				HumanBodyBones.RightShoulder,
				HumanBodyBones.LeftUpperArm,
				HumanBodyBones.RightUpperArm,
				HumanBodyBones.LeftLowerArm,
				HumanBodyBones.RightLowerArm,
				HumanBodyBones.LeftHand,
				HumanBodyBones.RightHand,
				HumanBodyBones.LeftThumbProximal,
				HumanBodyBones.LeftThumbIntermediate,
				HumanBodyBones.LeftThumbDistal,
				HumanBodyBones.LeftIndexProximal,
				HumanBodyBones.LeftIndexIntermediate,
				HumanBodyBones.LeftIndexDistal,
				HumanBodyBones.LeftMiddleProximal,
				HumanBodyBones.LeftMiddleIntermediate,
				HumanBodyBones.LeftMiddleDistal,
				HumanBodyBones.LeftRingProximal,
				HumanBodyBones.LeftRingIntermediate,
				HumanBodyBones.LeftRingDistal,
				HumanBodyBones.LeftLittleProximal,
				HumanBodyBones.LeftLittleIntermediate,
				HumanBodyBones.LeftLittleDistal,
				HumanBodyBones.RightThumbProximal,
				HumanBodyBones.RightThumbIntermediate,
				HumanBodyBones.RightThumbDistal,
				HumanBodyBones.RightIndexProximal,
				HumanBodyBones.RightIndexIntermediate,
				HumanBodyBones.RightIndexDistal,
				HumanBodyBones.RightMiddleProximal,
				HumanBodyBones.RightMiddleIntermediate,
				HumanBodyBones.RightMiddleDistal,
				HumanBodyBones.RightRingProximal,
				HumanBodyBones.RightRingIntermediate,
				HumanBodyBones.RightRingDistal,
				HumanBodyBones.RightLittleProximal,
				HumanBodyBones.RightLittleIntermediate,
				HumanBodyBones.RightLittleDistal,
		};

		public SmcZeroPressHandler()
		{
		}

		public void Setup(SMConverter converter, GameObject thisObject, GameObject other)
		{
			_converter = converter;
			MakeZeroShapes(thisObject, other);
			Update();
		}

		private void MakeZeroShapes(GameObject thisObject, GameObject other)
		{
			_bonesDet = new BonesDetails();
			_bonesDet.meshRenderers1 = thisObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			_bonesDet.meshRenderers2 = other.GetComponentsInChildren<SkinnedMeshRenderer>();
			_bonesDet.animator1 = thisObject.GetComponentInChildren<Animator>();
			_bonesDet.animator2 = other.GetComponentInChildren<Animator>();

			var posResult = new Vector3?[_bones.Length];
			var bones = new List<BoneDetails>(_bones.Length);

			for (int i = 0; i < _bones.Length; i++)
			{
				HumanBodyBones boneType = _bones[i];
				var bone1 = _bonesDet.animator1.GetBoneTransform(boneType);
				var bone2 = _bonesDet.animator2.GetBoneTransform(boneType);

				if (bone1 == null | bone2 == null)
				{
					posResult[i] = null;
					continue;
				}

				BoneDetails bd;
				bd.bone1 = bone1;
				bd.bone2 = bone2;
				bd.bone1Pointer = GetBoneDirPointer(bone1, boneType, _bonesDet.animator1);
				bd.bone2Pointer = GetBoneDirPointer(bone2, boneType, _bonesDet.animator2);
				bones.Add(bd);
			}
			_bonesDet.bones = bones.ToArray();

			MakeZeroShape(_bonesDet.meshRenderers1);
			MakeZeroShape(_bonesDet.meshRenderers2);
		}

		public void LateUpdate()
		{
			_bonesDet.animator1.Update(0);
			_bonesDet.animator2.Update(0);

			var bones = _bonesDet.bones;

			var posDatas = new PositionData[_bones.Length];
			for (int i = 0; i < bones.Length; i++)
			{
				var bone = bones[i];
				PositionData aa;
				aa.pos1 = bone.bone1.position;
				aa.pos2 = bone.bone2.position;
				aa.rot1 = bone.bone1.rotation;
				aa.rot2 = bone.bone2.rotation;
				if (bone.bone1Pointer != null & bone.bone2Pointer != null)
				{
					aa.pos1Pointer = bone.bone1Pointer.position;
					aa.pos2Pointer = bone.bone2Pointer.position;
				}
				else
				{
					aa.pos1Pointer = Vector3.zero;
					aa.pos2Pointer = Vector3.zero;
				}
				posDatas[i] = aa;
			}

			for (int i = 0; i < bones.Length; i++)
			{
				var bone = bones[i];
				var posDat = posDatas[i];

				Vector3 pos1 = posDat.pos1;
				Vector3 pos2 = posDat.pos2;

				Vector3 newPos = Vector3.Lerp(pos1, pos2, _converter.BlendRate);
				if (bone.bone1Pointer != null & bone.bone2Pointer != null)
				{
					var dir1 = (posDat.pos1Pointer - pos1).normalized;
					var dir2 = (posDat.pos2Pointer - pos2).normalized;
					var rDeltaTotal = Quaternion.FromToRotation(dir1, dir2);
					var rDelta = Quaternion.Lerp(Quaternion.identity, rDeltaTotal, _converter.BlendRate);

					bone.bone1.rotation = rDelta * posDat.rot1;
					bone.bone2.rotation = Quaternion.Inverse(rDeltaTotal) * rDelta * posDat.rot2;
				}
				bone.bone1.position = newPos;
				bone.bone2.position = newPos;
			}
		}

		public void Update()
		{
			for (int i = 0; i < _bonesDet.meshRenderers1.Length; i++)
			{
				var meshRenderer = _bonesDet.meshRenderers1[i];
				int index1 = meshRenderer.sharedMesh.GetBlendShapeIndex(ZeroWidthKey);
				meshRenderer.SetBlendShapeWeight(index1, Mathf.Clamp01(_converter.BlendRate * 2f - 1f));
			}

			for (int i = 0; i < _bonesDet.meshRenderers2.Length; i++)
			{
				var meshRenderer = _bonesDet.meshRenderers2[i];
				int index2 = meshRenderer.sharedMesh.GetBlendShapeIndex(ZeroWidthKey);
				meshRenderer.SetBlendShapeWeight(index2, Mathf.Clamp01(1f - _converter.BlendRate * 2f));
			}
		}

		public void Finished()
		{
		}

		private static Transform GetBoneDirPointer(Transform bone, HumanBodyBones boneType, Animator animator)
		{
			Transform result = null;

			// spine
			if (boneType == HumanBodyBones.Hips)
			{
				result = animator.GetBoneTransform(HumanBodyBones.Spine);
			}
			else if (boneType == HumanBodyBones.Spine)
			{
				result =
					animator.GetBoneTransform(HumanBodyBones.Chest) ??
					animator.GetBoneTransform(HumanBodyBones.UpperChest) ??
					animator.GetBoneTransform(HumanBodyBones.Neck) ??
					animator.GetBoneTransform(HumanBodyBones.Head);
			}
			else if (boneType == HumanBodyBones.Chest)
			{
				result =
					animator.GetBoneTransform(HumanBodyBones.UpperChest) ??
					animator.GetBoneTransform(HumanBodyBones.Neck) ??
					animator.GetBoneTransform(HumanBodyBones.Head);
			}
			else if (boneType == HumanBodyBones.UpperChest)
			{
				result =
					animator.GetBoneTransform(HumanBodyBones.Neck) ??
					animator.GetBoneTransform(HumanBodyBones.Head);
			}
			else if (boneType == HumanBodyBones.Neck)
			{
				result = animator.GetBoneTransform(HumanBodyBones.Head);
			}

			// left leg
			else if (boneType == HumanBodyBones.LeftUpperLeg)
			{
				result = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
			}
			else if (boneType == HumanBodyBones.LeftLowerLeg)
			{
				result = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			}
			else if (boneType == HumanBodyBones.LeftFoot)
			{
				result = animator.GetBoneTransform(HumanBodyBones.LeftToes);
			}

			// right leg
			else if (boneType == HumanBodyBones.RightUpperLeg)
			{
				result = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
			}
			else if (boneType == HumanBodyBones.RightLowerLeg)
			{
				result = animator.GetBoneTransform(HumanBodyBones.RightFoot);
			}
			else if (boneType == HumanBodyBones.RightFoot)
			{
				result = animator.GetBoneTransform(HumanBodyBones.RightToes);
			}

			// left hand
			else if (boneType == HumanBodyBones.LeftShoulder)
			{
				result = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
			}
			else if (boneType == HumanBodyBones.LeftUpperArm)
			{
				result = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
			}
			else if (boneType == HumanBodyBones.LeftLowerArm)
			{
				result = animator.GetBoneTransform(HumanBodyBones.LeftHand);
			}
			else if (boneType == HumanBodyBones.LeftHand)
			{
				result = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
			}

			// right hand
			else if (boneType == HumanBodyBones.RightShoulder)
			{
				result = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
			}
			else if (boneType == HumanBodyBones.RightUpperArm)
			{
				result = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
			}
			else if (boneType == HumanBodyBones.RightLowerArm)
			{
				result = animator.GetBoneTransform(HumanBodyBones.RightHand);
			}
			else if (boneType == HumanBodyBones.RightHand)
			{
				result = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
			}

			if (result != null)
			{
				return result;
			}

			float dist = float.MinValue;
			for (int i = 0; i < bone.childCount; i++)
			{
				var child = bone.GetChild(i);
				float cDist = child.localPosition.sqrMagnitude;
				if (cDist > dist)
				{
					dist = cDist;
					result = child;
				}
			}

			return result;
		}

		private static void MakeZeroShape(SkinnedMeshRenderer[] meshRenderers)
		{
			for (int i = 0; i < meshRenderers.Length; i++)
			{
				MakeZeroShape(meshRenderers[i]);
			}
		}

		private static void MakeZeroShape(SkinnedMeshRenderer meshRenderer)
		{
			int index = meshRenderer.sharedMesh.GetBlendShapeIndex(ZeroWidthKey);
			if (index != -1)
			{
				return;
			}
#if DEBUG
			var mesh = meshRenderer.sharedMesh = UnityEngine.Object.Instantiate(meshRenderer.sharedMesh); // UNITY bug workaround. BlendShapes do not reset after play finished in editor.
#else
			var mesh = meshRenderer.sharedMesh;
#endif

			var vertexes = mesh.vertices;
			var vertexesDelta = new Vector3[vertexes.Length];
			var boneWeights = mesh.boneWeights;
			var bonePos = mesh.bindposes.Select(p => p.inverse.MultiplyPoint3x4(Vector3.zero)).ToArray();


			for (int i = 0; i < vertexes.Length; ++i)
			{
				Vector3 v = vertexes[i];
				BoneWeight w = boneWeights[i];
				Vector3 vOrigin = GetOrigin(ref w, bonePos);

				vertexesDelta[i] = vOrigin - v;
			}

			mesh.AddBlendShapeFrame(ZeroWidthKey, 1f, vertexesDelta, null, null);
		}

		private static Vector3 GetOrigin(ref BoneWeight weightTo, Vector3[] bonePos)
		{
			return
				bonePos[weightTo.boneIndex0] * weightTo.weight0 +
				bonePos[weightTo.boneIndex1] * weightTo.weight1 +
				bonePos[weightTo.boneIndex2] * weightTo.weight2 +
				bonePos[weightTo.boneIndex3] * weightTo.weight3;
		}

		struct PositionData
		{
			public Vector3 pos1;
			public Vector3 pos2;
			public Vector3 pos1Pointer;
			public Vector3 pos2Pointer;
			public Quaternion rot1;
			public Quaternion rot2;
		}

		class BonesDetails
		{
			public Animator animator1;
			public Animator animator2;
			public BoneDetails[] bones;
			public SkinnedMeshRenderer[] meshRenderers1;
			public SkinnedMeshRenderer[] meshRenderers2;
		}

		struct BoneDetails
		{
			public Transform bone1;
			public Transform bone1Pointer;
			public Transform bone2;
			public Transform bone2Pointer;
		}
	}
}