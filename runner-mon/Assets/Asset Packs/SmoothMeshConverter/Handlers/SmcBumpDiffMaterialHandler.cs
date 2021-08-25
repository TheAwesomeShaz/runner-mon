using System.Collections.Generic;
using UnityEngine;

namespace BzKovSoft.SmoothMeshConverter.Handlers
{
	public class SmcBumpDiffMaterialHandler : ISmcHandler
	{
		private float _maxTextureRate = 1f;
		private float _noiseScale = 0.04f;
		private readonly Texture _substitudeTexture;
		private readonly Texture _substitudeNormalMap;
		private SMConverter _converter;

		List<MaterialBackup> _backups;

		public float MaxTextureRate { get => _maxTextureRate; set => _maxTextureRate = value; }
		public float NoiseScale { get => _noiseScale; set => _noiseScale = value; }

		public SmcBumpDiffMaterialHandler(Texture substitudeTexture, Texture substitudeNormalMap)
		{
			_substitudeTexture = substitudeTexture;
			_substitudeNormalMap = substitudeNormalMap;
		}

		public void Setup(SMConverter converter, GameObject thisObject, GameObject other)
		{
			_converter = converter;
			var renderers = new List<Renderer>();
			thisObject.GetComponentsInChildren<Renderer>(renderers);
			renderers.AddRange(other.GetComponentsInChildren<Renderer>());

			_backups = new List<MaterialBackup>(renderers.Count);

			foreach (var renderer in renderers)
			{
				var backup = new MaterialBackup();
				backup.renderer = renderer;
				backup.oldMaterials = renderer.sharedMaterials;
				backup.newMaterials = new Material[backup.oldMaterials.Length];
				_backups.Add(backup);

				for (int i = 0; i < backup.oldMaterials.Length; i++)
				{
					var oldMat = backup.oldMaterials[i];
					var newMat = new Material(Shader.Find("Custom/BzKovSoft/FadeToDiffTex"));
					newMat.SetTexture("_MainTex", oldMat.mainTexture);
					newMat.SetTexture("_SubstituteTex", _substitudeTexture);
					newMat.SetTexture("_SubstituteBumpMap", _substitudeNormalMap);
					if (oldMat.HasProperty("_Glossiness"))
					{
						newMat.SetFloat("_Glossiness", oldMat.GetFloat("_Glossiness"));
					}
					if (oldMat.HasProperty("_Metallic"))
					{
						newMat.SetFloat("_Metallic", oldMat.GetFloat("_Metallic"));
					}

					newMat.SetFloat("_NoiseFrequency", 5);

					backup.newMaterials[i] = newMat;
				}

				renderer.sharedMaterials = backup.newMaterials;
			}
		}

		public void Update()
		{
			float r = 1f - Mathf.Abs(_converter.BlendRate * 2f - 1f);
			float rate = r * _maxTextureRate;
			float noiseScale = r * _noiseScale;

			foreach (var backup in _backups)
			{
				for (int i = 0; i < backup.newMaterials.Length; i++)
				{
					var material = backup.newMaterials[i];
					material.SetFloat("_Rate", rate);
					material.SetFloat("_NoiseScale", noiseScale);
				}
			}
		}

		public void LateUpdate()
		{
		}

		public void Finished()
		{
			foreach (var backup in _backups)
			{
				backup.renderer.sharedMaterials = backup.oldMaterials;
			}
		}

		struct MaterialBackup
		{
			public Material[] oldMaterials;
			public Material[] newMaterials;
			public Renderer renderer;
		}
	}
}
