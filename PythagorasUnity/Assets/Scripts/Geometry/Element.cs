using UnityEngine;
using System.Collections;

/*
	Base class for geometric elements
*/

[RequireComponent( typeof( MeshFilter ) )]
[RequireComponent( typeof( MeshRenderer ) )]
public abstract class Element : MonoBehaviour
{
	public readonly static bool DEBUG_ELEMENT = true;
	public readonly static bool DEBUG_ELEMENT_VERBOSE = true;

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

	private bool isDirty_ = false;
	protected void SetDirty()
	{
		isDirty_ = true;
	}

	#endregion private data

	#region in-editor modding 

#if UNITY_EDITOR
	// for in-editor modification

	abstract protected void CheckIfModded( );
	abstract protected void SetModdingValues( );

#endif

	#endregion in-editor modding 

	#region interface

	abstract protected void DoAdjustMesh( );
	
	#endregion interface

	#region MB Flow

	private void Awake()
	{
		cachedTransform_ = transform;
		cachedMeshFilter_ = GetComponent<MeshFilter>( );
		cachedMeshRenderer_ = GetComponent<MeshRenderer>( );
		cachedMaterial_ = new Material( cachedMeshRenderer_.sharedMaterial );
		cachedMeshRenderer_.material = cachedMaterial_;

		PostAwake( );
	}

	private void Update( )
	{
		if (isDirty_)
		{
			AdjustMesh( );
		}
#if UNITY_EDITOR
		CheckIfModded( );
#endif
	}


	#endregion MB Flow

	#region Setup

	private void AdjustMesh()
	{
		DoAdjustMesh( );
#if UNITY_EDITOR
		SetModdingValues( );
#endif
		isDirty_ = false;
	}

	// Call this from derived classes' Init functions
	protected void Init( Field f, float d )
	{
		SetField( f );
		depth_ = d;
		SetDirty( );
	}

	private void SetField(Field f)
	{
		field_ = f;
		cachedTransform_.SetParent( field_.cachedTransform );
		cachedTransform_.localScale = Vector3.one;
		cachedTransform_.localRotation = Quaternion.identity;
		cachedTransform_.localPosition = Vector3.zero;
		SetDirty( );
	}

	// Called from Awake(), override in derived classes for additional functionality
	protected virtual void PostAwake( ) { }

	public void SetDepth(float d)
	{
		depth_ = d;
		SetDirty( );
	}

	#endregion Setup

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

	#region Creation

	public T Clone<T>( string name, float d ) where T :Element
	{
		GameObject go = GameObject.Instantiate( this.gameObject ) as GameObject;
		go.name = name;
		T component = go.GetComponent<T>( );
		if (component == null)
		{
			throw new System.Exception( "Attempt to clone " + gameObject.name + ", which is a " + this.GetType( ).ToString( ) + " as a " + typeof( T ).ToString( ) );
		}
		if (DEBUG_ELEMENT)
		{
			Debug.Log( "Cloned  "+this.GetType().ToString()+" '" + gameObject.name + "' as a " + typeof( T ).ToString( ) + " called '" + name +"'");
		}
		component.cachedMeshFilter_.mesh = Mesh.Instantiate( cachedMeshFilter_.sharedMesh );
		component.SetField( field_ );
		component.SetDepth( d );
		component.OnClone( this );
		return component;
	}

	public T Clone<T>( string name ) where T : Element
	{
		return Clone<T>( name, depth_ );
	}

	// Note - have done it this way tp leave possibility of cloning from other types (eg Quadrilateral from parallelogram from square, etc)
	protected abstract void OnClone<T>( T src) where T :Element;

	#endregion Creation
	}
