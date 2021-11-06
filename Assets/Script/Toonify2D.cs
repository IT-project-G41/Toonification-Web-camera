using System.Collections;
using UnityEngine;

public class Toonify2D : MonoBehaviour
{
	public Shader gaussianBlurShader;   // declare a shader 
	private Material gaussianBlurMaterial = null;   // declare a meterial


	// Called when need to create the material used by this effect
	protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
	{
		if (shader == null)
		{
			return null;
		}

		if (shader.isSupported && material && material.shader == shader)
			return material;

		if (!shader.isSupported)
		{
			return null;
		}
		else
		{
			material = new Material(shader);
			material.hideFlags = HideFlags.DontSave;
			if (material)
				return material;
			else
				return null;
		}
	}


	// Call the CheckShaderAndCreateMaterial function to create a material
	public Material material
	{
		get
		{
			gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader, gaussianBlurMaterial);
			return gaussianBlurMaterial;
		}
	}



	[Range(0.0f, 3.0f)]
	public float brightness = 1.7f;

	[Range(0.0f, 3.0f)]
	public float saturation = 1.0f;

	[Range(0.0f, 3.0f)]
	public float contrast = 1.0f;


	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (material != null)
		{

			//Graphics.Blit(src, dest, material);

			material.SetFloat("_Brightness", brightness);
			material.SetFloat("_Satruation", saturation);
			material.SetFloat("_Constart", contrast);


			int rtW = src.width;
			int rtH = src.height;

			// define the fisrt cache area buffer0
			RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
			buffer0.filterMode = FilterMode.Bilinear;

			// Two caches are used to render the two passes separately
			Graphics.Blit(src, buffer0);
			for (int i = 0; i < 1; i++)
			{

				// define the second cache area buffer1
				RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

				// render the first pass
				Graphics.Blit(buffer0, buffer1, material, 0);

				// release the first cache area and replace it 
				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
				buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

				// render the second pass
				Graphics.Blit(buffer0, buffer1, material, 1);


				RenderTexture.ReleaseTemporary(buffer0);
				buffer0 = buffer1;
			}
			Graphics.Blit(buffer0, dest);
			RenderTexture.ReleaseTemporary(buffer0);
		}
		else
		{
			Graphics.Blit(src, dest);
		}
	}
}
