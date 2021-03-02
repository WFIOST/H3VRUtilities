using System;
using FistVR;
using UnityEngine;
using UnityEngine.VR;

public class ScopeCamMagnifier : MonoBehaviour
{
	public bool MagnificationEnabled
	{
		get
		{
			return this.m_magnifcationEnabled;
		}
		set
		{
			if (this.m_magnifcationEnabled != value)
			{
				this.m_magnifcationEnabled = value;
				if (this.m_magnifcationEnabled)
				{
					if (this.Reticule != null)
					{
						this.Reticule.SetActive(true);
					}
				}
				else if (this.Reticule != null)
				{
					this.Reticule.SetActive(false);
				}
				if (!this.m_magnifcationEnabled && this.m_mainTex != null)
				{
					this.ClearToBlack(this.m_mainTex);
					this.ClearToBlack(this.m_blurTex);
				}
			}
		}
	}

	private void ClearToBlack(RenderTexture tex)
	{
		if (tex != null)
		{
			RenderTexture.active = tex;
			GL.Clear(false, true, Color.black);
			RenderTexture.active = null;
		}
	}

	private void OnEnable()
	{
		this.m_mainTex = new RenderTexture(this.Resolution, this.Resolution, 24, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.sRGB)
		{
			wrapMode = TextureWrapMode.Clamp
		};
		this.m_blurTex = new RenderTexture(this.Resolution, this.Resolution, 0, this.m_mainTex.format, RenderTextureReadWrite.sRGB);
		this.m_mainTex.Create();
		this.m_blurTex.Create();
		this.ScopeCamera.enabled = false;
		this.ScopeCamera.allowHDR = true;
		this.ScopeCamera.allowMSAA = false;
		this.m_renderer = base.GetComponent<Renderer>();
		this.m_block = new MaterialPropertyBlock();
		if (this.Reticule != null)
		{
			this.m_reticuleSize = this.Reticule.transform.localScale;
		}
		this.MagnificationEnabled = this.MagnificationEnabledAtStart;
	}

	private void OnWillRenderObject()
	{
		if (Camera.current == this.ScopeCamera || !this.MagnificationEnabled)
		{
			return;
		}
		Vector3 position = this.m_renderer.bounds.min;
		Vector3 position2 = this.m_renderer.bounds.max;
		position = this.m_renderer.transform.position - Camera.current.transform.right * this.m_renderer.transform.localScale.x - Camera.current.transform.up * this.m_renderer.transform.localScale.x;
		position2 = this.m_renderer.transform.position + Camera.current.transform.right * this.m_renderer.transform.localScale.x + Camera.current.transform.up * this.m_renderer.transform.localScale.x;
		Vector3 vector = Camera.current.WorldToViewportPoint(position);
		Vector3 vector2 = Camera.current.WorldToViewportPoint(position2);
		float num = Mathf.Abs(Mathf.Clamp01(vector2.x) - Mathf.Clamp01(vector.x));
		float num2 = Mathf.Abs(Mathf.Clamp01(vector2.y) - Mathf.Clamp01(vector.y));
		float num3 = Mathf.Sqrt(num * num + num2 * num2);
		this.ScopeCamera.fieldOfView = Camera.current.fieldOfView * num3 / (this.Magnification * 3.1415927f * 0.5f);
		Vector3 vector3;
		if (!Camera.current.stereoEnabled)
		{
			vector3 = Camera.current.transform.position;
			this.m_block.SetFloat("_EyeIndex", 0f);
		}
		else
		{
			Vector3 position3 = GM.CurrentPlayerBody.Head.position;
			Vector3 a = position3 + GM.CurrentPlayerBody.Head.right * -0.022f;
			Vector3 a2 = position3 + GM.CurrentPlayerBody.Head.right * 0.022f;
			Vector3 to = a - base.transform.position;
			Vector3 to2 = a2 - base.transform.position;
			float num4 = Vector3.Angle(base.transform.forward, to);
			float num5 = Vector3.Angle(base.transform.forward, to2);
			VRNode vrnode;
			if (num5 < num4)
			{
				vrnode = VRNode.LeftEye;
			}
			else
			{
				vrnode = VRNode.RightEye;
			}
			vector3 = Quaternion.Inverse(InputTracking.GetLocalRotation(vrnode)) * InputTracking.GetLocalPosition(vrnode);
			Matrix4x4 cameraToWorldMatrix = Camera.current.cameraToWorldMatrix;
			Vector3 v = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.Head)) * InputTracking.GetLocalPosition(VRNode.Head);
			vector3 = cameraToWorldMatrix.MultiplyPoint(vector3) + (Camera.current.transform.position - cameraToWorldMatrix.MultiplyPoint(v));
			this.m_block.SetFloat("_EyeIndex", (float)((vrnode != VRNode.LeftEye) ? 1 : 0));
		}
		if ((this.ScopeCamera.transform.position - vector3).magnitude >= Mathf.Epsilon)
		{
			this.ScopeCamera.targetTexture = this.m_mainTex;
			if (this.Reticule != null)
			{
				Transform transform = this.Reticule.transform;
				transform.position = this.ScopeCamera.transform.position + base.transform.forward * 0.1f;
				transform.rotation = base.transform.rotation;
			}
			this.ScopeCamera.Render();
			this.PostMaterial.SetVector("_CamPos", vector3);
			this.PostMaterial.SetMatrix("_ScopeVisualToWorld", base.transform.localToWorldMatrix);
			this.PostMaterial.SetVector("_Forward", this.ScopeCamera.transform.forward);
			this.PostMaterial.SetVector("_Offset", Vector2.right * this.AngleBlurStrength * 0.01f);
			Graphics.Blit(this.m_mainTex, this.m_blurTex, this.PostMaterial);
			this.PostMaterial.SetVector("_Offset", Vector2.up * this.AngleBlurStrength * 0.01f);
			Graphics.Blit(this.m_blurTex, this.m_mainTex, this.PostMaterial);
		}
		this.m_block.SetVector("_TubeCenter", base.transform.position);
		this.m_block.SetVector("_TubeForward", base.transform.forward);
		this.m_block.SetFloat("_TubeRadius", base.transform.localScale.x);
		float num6 = (this.ScopeCamera.transform.position - base.transform.position).magnitude;
		num6 *= Mathf.Lerp(1f, this.Magnification, this.AngularOccludeSensitivity);
		this.m_block.SetFloat("_TubeLength", num6);
		this.m_block.SetFloat("_CutoffSoftness", this.CutoffSoftness);
		this.m_block.SetFloat("_LensDistortion", 1f - this.LensSpaceDistortion);
		this.m_block.SetFloat("_Chroma", this.LensChromaticDistortion);
		this.m_block.SetTexture("_MainTex0", this.m_mainTex);
		this.m_renderer.SetPropertyBlock(this.m_block);
	}

	public void PointTowards(Vector3 p)
	{
		Vector3 vector = p - this.ScopeCamera.transform.position;
		vector = Vector3.ProjectOnPlane(vector, base.transform.right);
		this.ScopeCamera.transform.rotation = Quaternion.LookRotation(vector, base.transform.up);
	}

	private void RenderScopeTex(VRNode node, RenderTexture tex)
	{
		Vector3 vector = Quaternion.Inverse(InputTracking.GetLocalRotation(node)) * InputTracking.GetLocalPosition(node);
		Matrix4x4 cameraToWorldMatrix = Camera.current.cameraToWorldMatrix;
		Vector3 v = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.Head)) * InputTracking.GetLocalPosition(VRNode.Head);
		vector = cameraToWorldMatrix.MultiplyPoint(vector) + (Camera.current.transform.position - cameraToWorldMatrix.MultiplyPoint(v));
		Vector3 vector2 = this.ScopeCamera.transform.position - vector;
		if (vector2.magnitude < Mathf.Epsilon)
		{
			return;
		}
		Quaternion quaternion = Quaternion.LookRotation(vector2.normalized, base.transform.up);
		this.ScopeCamera.targetTexture = tex;
		if (this.Reticule != null)
		{
			Transform transform = this.Reticule.transform;
			transform.position = this.ScopeCamera.transform.position + base.transform.forward * 0.1f;
			transform.rotation = base.transform.rotation;
		}
		this.ScopeCamera.Render();
		this.PostMaterial.SetVector("_CamPos", vector);
		this.PostMaterial.SetMatrix("_ScopeVisualToWorld", base.transform.localToWorldMatrix);
		this.PostMaterial.SetVector("_Forward", this.ScopeCamera.transform.forward);
		this.PostMaterial.SetVector("_Offset", Vector2.right * this.AngleBlurStrength * 0.01f);
		Graphics.Blit(tex, this.m_blurTex, this.PostMaterial);
		this.PostMaterial.SetVector("_Offset", Vector2.up * this.AngleBlurStrength * 0.01f);
		Graphics.Blit(this.m_blurTex, tex, this.PostMaterial);
	}

	private void OnDisable()
	{
		UnityEngine.Object.DestroyImmediate(this.m_mainTex);
		UnityEngine.Object.DestroyImmediate(this.m_blurTex);
	}

	public Material PostMaterial;

	public GameObject Reticule;

	public Camera ScopeCamera;

	public float Magnification = 5f;

	public int Resolution = 512;

	public float AngleBlurStrength = 0.5f;

	public float CutoffSoftness = 0.05f;

	public float AngularOccludeSensitivity = 0.5f;

	public float ReticuleScale = 1f;

	public bool MagnificationEnabledAtStart;
	[Range(0f, 1f)]
	public float LensSpaceDistortion = 0.075f;
	[Range(0f, 5f)]
	public float LensChromaticDistortion = 0.075f;
	private Renderer m_renderer;
	private MaterialPropertyBlock m_block;
	private Vector3 m_reticuleSize = new Vector3(0.1f, 0.1f, 0.1f);
	private RenderTexture m_mainTex;
	private RenderTexture m_blurTex;
	private bool m_magnifcationEnabled;
}
