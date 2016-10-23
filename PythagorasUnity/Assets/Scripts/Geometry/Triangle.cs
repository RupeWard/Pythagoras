using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ RequireComponent ( typeof( MeshFilter ))]
[ RequireComponent ( typeof( MeshRenderer ) )]
public class Triangle : MonoBehaviour
{
	#region inspector hooks
	#endregion inspector hooks

	#region private hooks

	private Transform cachedTransform_ = null;
	public Transform cachedTransform
	{
		get { return cachedTransform_; }
	}
	private Material cachedMaterial_ = null;
	private MeshRenderer cachedMeshRenderer_ = null;
	private MeshFilter cachedMeshFilter_ = null;

	private Field field_ = null; // TODO do we need this? If removing, need another way to check if initialised

	private float depth_ = 0f;

	#endregion private hooks

	#region private data

	private List< Vector2 > vertices_ = new List<Vector2>( 3 ) { Vector3.zero, Vector3.zero, Vector3.zero };

	private static readonly Vector3 s_normal = new Vector3( 0f, 0f, -1f );

#if UNITY_EDITOR

	public Vector2[] modVertices = new Vector2[3];

	private void AdjustIfModded()
	{
		bool doAdjust = false;
		for (int i = 0; i<3; i++)
		{
			if (modVertices[i] != vertices_[i])
			{
				vertices_[i] = modVertices[i];
				doAdjust = true;
			}
		}
		if (doAdjust)
		{
			Adjust( );
		}
	}
#endif

	#endregion private data

	#region MB Flow

	private void Awake()
	{
		cachedTransform_ = transform;
		cachedMeshFilter_ = GetComponent<MeshFilter>( );
		cachedMeshRenderer_ = GetComponent<MeshRenderer>( );
		cachedMaterial_ = new Material( cachedMeshRenderer_.sharedMaterial );
		cachedMeshRenderer_.material = cachedMaterial_;

		for (int i = 0; i<3; i++)
		{
			modVertices[i] = vertices_[i];
		}
	}

	private void Start ()
	{
	
	}
	
	private void Update ()
	{
#if UNITY_EDITOR
		AdjustIfModded( );
#endif
	}

	public void Init(Field f, Vector2[] vs, float d)
	{
		if (vs.Length != 3)
		{
			throw new System.Exception( "vs.Length should be 3" );
		}

		field_ = f;

		cachedTransform_.SetParent( field_.cachedTransform );
		cachedTransform_.localScale = Vector3.one;
		cachedTransform_.localRotation = Quaternion.identity;

		depth_ = d;
		for (int i = 0; i<3; i++)
		{
			vertices_[i] = vs[i];
		}

		Adjust( );
	}

	#endregion MB Flow

	#region Mesh

	public void Adjust()
	{
		if (field_ == null) // Don't make mesh if not initialised
		{
			return;
		}

#if UNITY_EDITOR
		for (int i = 0; i < 3; i++)
		{
			modVertices[i] = vertices_[i];
		}
#endif
		Mesh mesh = cachedMeshFilter_.sharedMesh;
		if (mesh == null)
		{
			cachedMeshFilter_.sharedMesh = new Mesh( );
			mesh = cachedMeshFilter_.sharedMesh;
		}

		Vector3[] verts = new Vector3[3];
		Vector2[] uvs = new Vector2[3];
		Vector3[] normals = new Vector3[3];

		for (int i = 0; i < 3; i++)
		{
			verts[i] = new Vector3( vertices_[i].x, vertices_[i].y, depth_ );
			uvs[i] = Vector2.zero;
			normals[i] = s_normal;
		}

		mesh.vertices = verts;
		mesh.triangles = new int[3] { 0, 1, 2 };
		mesh.uv = uvs;
		mesh.normals = normals;

		mesh.RecalculateBounds( );
		mesh.Optimize( );

	}

	#endregion Mesh
}
