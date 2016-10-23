using UnityEngine;
using System.Collections;

/*
	Base class for geometric elements
*/

[RequireComponent( typeof( MeshFilter ) )]
[RequireComponent( typeof( MeshRenderer ) )]
public abstract class Element : MonoBehaviour
{
	#region private hooks

	private Transform cachedTransform_ = null;
	public Transform cachedTransform
	{
		get { return cachedTransform_; }
	}
	private Material cachedMaterial_ = null;
	public Material cachedMaterial
	{
		get { return cachedMaterial_;  }
	}
	private MeshRenderer cachedMeshRenderer_ = null;
	private MeshFilter cachedMeshFilter_ = null;
	
	#endregion private hooks

	#region private data

	protected static readonly Vector3 s_normal = new Vector3( 0f, 0f, -1f );

	private Field field_ = null; // TODO do we need this? If removing, need another way to check if initialised
	protected Field field
	{
		get { return field_;  }
	}

	private float depth_ = 0f; // Relative to field. Lower depth elements appear in front. Overlapping same-depth elements may cause z-fighting
	protected float depth
	{
		get { return depth_;  }
	}

	#endregion private data

	#region Flow

	private void Awake()
	{
		cachedTransform_ = transform;
		cachedMeshFilter_ = GetComponent<MeshFilter>( );
		cachedMeshRenderer_ = GetComponent<MeshRenderer>( );
		cachedMaterial_ = new Material( cachedMeshRenderer_.sharedMaterial );
		cachedMeshRenderer_.material = cachedMaterial_;

		PostAwake( );
	}

	// Call this from derived classes' Init functions
	protected void Init( Field f, float d )
	{
		field_ = f;
		depth_ = d;

		cachedTransform_.SetParent( field_.cachedTransform );
		cachedTransform_.localScale = Vector3.one;
		cachedTransform_.localRotation = Quaternion.identity;
	}

	// Called from Awake(), override in derived classes for additional functionality
	protected virtual void PostAwake( ) { }

	#endregion Flow

	#region Mesh

	// Creates the mesh if it doesn't yet exist
	protected Mesh GetMesh()
	{
		Mesh mesh = cachedMeshFilter_.sharedMesh;
		if (mesh == null)
		{
			cachedMeshFilter_.sharedMesh = new Mesh( );
			mesh = cachedMeshFilter_.sharedMesh;
		}
		return mesh;
	}

	#endregion
}
