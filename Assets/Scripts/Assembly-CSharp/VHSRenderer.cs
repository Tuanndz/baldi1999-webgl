using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public sealed class VHSRenderer : PostProcessEffectRenderer<VHS>
{
	private class VHSState
	{
		public float horizontalNoisePos;
	}

	private RenderTexture[] blurPyramid;

	private RenderTexture noiseBuffer;

	private RenderTexture noiseBuffer2;

	private Texture2D _grainTex;

	private Texture2D _horizontalNoiseTex;

	private Texture2D _speckNoiseTex;

	private Shader _shader_downsample;

	private Shader _shader_noiseGen;

	private Shader _shader_composite;

	private static Dictionary<Camera, VHSState> cameraStates = new Dictionary<Camera, VHSState>();

	private Texture2D grainTex
	{
		get
		{
			if (_grainTex == null)
			{
				_grainTex = (Texture2D)Resources.Load("vhsGrain", typeof(Texture2D));
			}
			return _grainTex;
		}
	}

	private Texture2D horizontalNoiseTex
	{
		get
		{
			if (_horizontalNoiseTex == null)
			{
				_horizontalNoiseTex = (Texture2D)Resources.Load("horizontalNoise", typeof(Texture2D));
			}
			return _horizontalNoiseTex;
		}
	}

	private Texture2D speckNoiseTex
	{
		get
		{
			if (_speckNoiseTex == null)
			{
				_speckNoiseTex = (Texture2D)Resources.Load("speckNoise", typeof(Texture2D));
			}
			return _speckNoiseTex;
		}
	}

	private Shader shader_downsample
	{
		get
		{
			if (_shader_downsample == null)
			{
				_shader_downsample = Shader.Find("Hidden/VHSDownsample");
			}
			return _shader_downsample;
		}
	}

	private Shader shader_noiseGen
	{
		get
		{
			if (_shader_noiseGen == null)
			{
				_shader_noiseGen = Shader.Find("Hidden/VHSNoiseGen");
			}
			return _shader_noiseGen;
		}
	}

	private Shader shader_composite
	{
		get
		{
			if (_shader_composite == null)
			{
				_shader_composite = Shader.Find("Hidden/VHSComposite");
			}
			return _shader_composite;
		}
	}

	private void AllocateTempRT(ref RenderTexture tex, int width, int height, RenderTextureFormat format)
	{
		if (tex != null && (tex.width != width || tex.height != height))
		{
			RenderTexture.ReleaseTemporary(tex);
			tex = null;
		}
		if (tex == null)
		{
			tex = RenderTexture.GetTemporary(width, height, 0, format);
		}
	}

	public override void Render(PostProcessRenderContext context)
	{
		if (context.width <= 0 || context.height <= 0 || speckNoiseTex == null || horizontalNoiseTex == null || grainTex == null)
		{
			return;
		}
		VHSState value;
		if (!cameraStates.TryGetValue(context.camera, out value))
		{
			value = new VHSState();
			cameraStates[context.camera] = value;
		}
		value.horizontalNoisePos += Time.deltaTime * 0.004f;
		if (Random.value < 0.01f)
		{
			value.horizontalNoisePos += Random.value;
		}
		value.horizontalNoisePos = Mathf.Repeat(value.horizontalNoisePos, 1f);
		int num = Mathf.Max(1, Mathf.Min(640, Mathf.RoundToInt((float)context.width * 0.5f)));
		int num2 = Mathf.Max(1, Mathf.Min(480, Mathf.RoundToInt((float)context.height * 0.5f)));
		AllocateTempRT(ref noiseBuffer, num, num2, RenderTextureFormat.R8);
		AllocateTempRT(ref noiseBuffer2, num, num2, RenderTextureFormat.R8);
		PropertySheet propertySheet = context.propertySheets.Get(shader_noiseGen);
		propertySheet.properties.SetTexture("_HorizontalNoise", horizontalNoiseTex);
		propertySheet.properties.SetFloat("_HorizontalNoisePos", value.horizontalNoisePos);
		propertySheet.properties.SetFloat("_HorizontalNoisePower", (float)base.settings.stripeNoiseDensity * (float)base.settings.stripeNoiseDensity);
		propertySheet.properties.SetTexture("_SpeckNoise", speckNoiseTex);
		propertySheet.properties.SetVector("_SpeckNoiseScaleOffset", new Vector4((float)num / (float)speckNoiseTex.width, (float)num2 / (float)speckNoiseTex.height, Random.value, Random.value));
		context.command.BlitFullscreenTriangle(Texture2D.blackTexture, noiseBuffer, propertySheet, 0);
		propertySheet.properties.SetVector("_SmearOffsetAttenuation", new Vector4(1f, 0.2f));
		context.command.BlitFullscreenTriangle(noiseBuffer, noiseBuffer2, propertySheet, 1);
		propertySheet.properties.SetVector("_SmearOffsetAttenuation", new Vector4(5f, 0.8f));
		context.command.BlitFullscreenTriangle(noiseBuffer2, noiseBuffer, propertySheet, 1);
		if (blurPyramid == null || blurPyramid.Length != (int)base.settings.colorBleedIterations)
		{
			blurPyramid = new RenderTexture[(int)base.settings.colorBleedIterations];
		}
		int num3 = context.width / 2;
		int num4 = context.height / 2;
		PropertySheet propertySheet2 = context.propertySheets.Get(shader_downsample);
		for (int i = 0; i < (int)base.settings.colorBleedIterations; i++)
		{
			num3 = Mathf.Max(1, num3 / 2);
			num4 = Mathf.Max(1, num4 / 2);
			if (blurPyramid[i] != null && (blurPyramid[i].width != num3 || blurPyramid[i].height != num4))
			{
				RenderTexture.ReleaseTemporary(blurPyramid[i]);
				blurPyramid[i] = null;
			}
			if (blurPyramid[i] == null)
			{
				blurPyramid[i] = RenderTexture.GetTemporary(num3, num4, 0, context.sourceFormat);
			}
			propertySheet2.properties.SetTexture("_Noise", (i == 0) ? ((Texture)noiseBuffer) : ((Texture)Texture2D.blackTexture));
			if (i == 0)
			{
				propertySheet2.properties.SetFloat("_NoiseOpacity", base.settings.stripeNoiseOpacity);
			}
			context.command.BlitFullscreenTriangle((i == 0) ? context.source : ((RenderTargetIdentifier)blurPyramid[i - 1]), blurPyramid[i], propertySheet2, 0);
		}
		for (int num5 = (int)base.settings.colorBleedIterations - 1; num5 > 1; num5--)
		{
			propertySheet2.properties.SetFloat("_UpsampleBlend", 0.6f);
			context.command.BlitFullscreenTriangle(blurPyramid[num5], blurPyramid[num5 - 1], propertySheet2, 1);
		}
		PropertySheet propertySheet3 = context.propertySheets.Get(shader_composite);
		propertySheet3.properties.SetFloat("_ColorBleedIntensity", base.settings.colorBleedingIntensity);
		propertySheet3.properties.SetTexture("_Grain", grainTex);
		propertySheet3.properties.SetFloat("_GrainIntensity", base.settings.grainIntensity);
		propertySheet3.properties.SetVector("_GrainScaleOffset", new Vector4(0.6f * (float)base.settings.grainScale, base.settings.grainScale, Random.value, Random.value));
		propertySheet3.properties.SetTexture("_Noise", noiseBuffer);
		propertySheet3.properties.SetFloat("_NoiseOpacity", base.settings.stripeNoiseOpacity);
		propertySheet3.properties.SetFloat("_EdgeIntensity", base.settings.edgeIntensity);
		propertySheet3.properties.SetFloat("_EdgeDistance", base.settings.edgeDistance);
		propertySheet3.properties.SetTexture("_SlightBlurredTex", blurPyramid[0]);
		propertySheet3.properties.SetTexture("_BlurredTex", blurPyramid[1]);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet3, 0);
	}

	public override void Release()
	{
		if (noiseBuffer != null)
		{
			RenderTexture.ReleaseTemporary(noiseBuffer);
			noiseBuffer = null;
		}
		if (noiseBuffer2 != null)
		{
			RenderTexture.ReleaseTemporary(noiseBuffer2);
			noiseBuffer2 = null;
		}
		if (blurPyramid != null)
		{
			for (int i = 0; i < blurPyramid.Length; i++)
			{
				if (blurPyramid[i] != null)
				{
					RenderTexture.ReleaseTemporary(blurPyramid[i]);
					blurPyramid[i] = null;
				}
			}
		}
		base.Release();
	}
}
