using UnityEngine;
using System.Collections;

/*
	Base class for geometric elements
*/

namespace RJWard.Geometry
{
	[RequireComponent( typeof( MeshFilter ) )]
	[RequireComponent( typeof( MeshRenderer ) )]
	public abstract class ElementBase : MonoBehaviour
	{
		public readonly static bool DEBUG_ELEMENT = true;
		public readonly static bool DEBUG_ELEMENT_VERBOSE = true;

		#region inspector data

		public bool useSharedMaterial = false;
		
		#endregion inspector data

		#region private hooks

		private Transform cachedTransform_ = null;
		public Transform cachedTransform
		{
			get
			{
				if (cachedTransform_ == null)
				{
					cachedTransform_ = transform;
				}
				return cachedTransform_;
			}
		}

		private Material cachedMaterial_ = null;
		public Material cachedMaterial
		{
			get
			{
				if (cachedMaterial_ == null)
				{
					cachedMaterial_ = (useSharedMaterial) ? (cachedMeshRenderer.sharedMaterial) : (cachedMeshRenderer.material);
					cachedMeshRenderer_.material = cachedMaterial_;
				}
				return cachedMaterial_;
			}
		}

		private MeshRenderer cachedMeshRenderer_ = null;
		public MeshRenderer cachedMeshRenderer
		{
			get
			{
				if (cachedMeshRenderer_ == null)
				{
					cachedMeshRenderer_ = GetComponent<MeshRenderer>( );
				}
				return cachedMeshRenderer_;
			}
		}

		private MeshFilter cachedMeshFilter_ = null;
		public MeshFilter cachedMeshFilter
		{
			get
			{
				if (cachedMeshFilter_ == null)
				{
					cachedMeshFilter_ = GetComponent<MeshFilter>( );
				}
				return cachedMeshFilter_;
			}
		}

		#endregion private hooks

		#region private data

		protected static readonly Vector3 s_normal = new Vector3( 0f, 0f, -1f );

		private GeometryFactory geometryFactory_ = null;
		protected GeometryFactory geometryFactory
		{
			get { return geometryFactory_; }
		}

		private Field field_ = null; // TODO do we need this? If removing, need another way to check if initialised
		public Field field
		{
			get { return field_; }
		}

		private float depth_ = 0f; // Relative to field. Lower depth elements appear in front. Overlapping same-depth elements may cause z-fighting
		public float depth
		{
			get { return depth_; }
		}

		private bool isMeshDirty_ = false;
		protected void SetMeshDirty( )
		{
			isMeshDirty_ = true;
		}

		private ElementDecoratorBase decorator_;
		protected ElementDecoratorBase decorator
		{
			get { return decorator_; }
			set { decorator_ = value; }
		}

		protected T Decorator< T >() where T : ElementDecoratorBase
		{
			return decorator_ as T;
		}

		/*
		private Color colour_; // TODO replace with some kind of decorator
		public Color colour
		{
			get { return colour_;  }
		}

		private float alpha_; // TODO replace with some kind of decorator
		public float alpha
		{
			get { return alpha_; }
		}
		*/

		#endregion private data

		#region Non-geometrical Appaarance

		abstract protected void HandleColourChanged( Color c);
		abstract protected void HandleAlphaChanged( float a);

		public void SetColour( Color c)
		{
			decorator.colour = c;
		}

		public virtual void SetAlpha( float a)
		{
			decorator.alpha = a;
		}

		/*
		public void SetColour( Color c )
		{
			if (colour_ != c)
			{
				colour_ = c;
				HandleColourChanged( );
			}
		}

		public void SetAlpha( float a )
		{
			if (alpha_ != a)
			{
				alpha_ = a;
				HandleAlphaChanged( );
			}
		}

		public void SetColour( Color c, float a )
		{
			SetColour( c );
			SetAlpha( a );
		}
		*/

		#endregion Non-geometrical Appaarance


		#region in-editor modding 

#if UNITY_EDITOR
		// for in-editor modification

		abstract protected void CheckIfModded( );
		abstract protected void SetModdingValues( );

#endif

		#endregion in-editor modding 

		#region MB Flow

		private void Awake( )
		{
			cachedTransform_ = transform;
			cachedMeshFilter_ = GetComponent<MeshFilter>( );
			cachedMeshRenderer_ = GetComponent<MeshRenderer>( );
			cachedMaterial_ = (useSharedMaterial)?(cachedMeshRenderer_.sharedMaterial):(cachedMeshRenderer_.material);
			cachedMeshRenderer_.material = cachedMaterial_;

			PostAwake( );
		}

		private void Update( )
		{
			if (isMeshDirty_)
			{
				AdjustMesh( );
			}
#if UNITY_EDITOR
			CheckIfModded( );
#endif
		}

		#endregion MB Flow

		#region Setup

		protected void SetSharedMaterial (Material m)
		{
			cachedMaterial_ = m;
			cachedMeshRenderer_.sharedMaterial = cachedMaterial_;
		}

		private void AdjustMesh( )
		{
			DoAdjustMesh( );
#if UNITY_EDITOR
			SetModdingValues( );
#endif
			isMeshDirty_ = false;
		}

		// Call this from derived classes' Init functions
		// override with exception in classes when required (eg 2D base class where the numVertices must be set)
		protected virtual void Init( GeometryFactory gf, Field f, float d, Material mat )
		{
			geometryFactory_ = gf;
			SetField( f );
			depth_ = d;
			if (mat != null)
			{
				useSharedMaterial = true;
				SetSharedMaterial( mat );
			}
			SetMeshDirty( );
		}

		protected virtual void Init( GeometryFactory gf, Field f, float d)
		{
			Init( gf, f, d, null );
		}

		private void SetField( Field f )
		{
			field_ = f;
			cachedTransform.SetParent( field_.cachedTransform );
			cachedTransform_.localScale = Vector3.one;
			cachedTransform_.localRotation = Quaternion.identity;
			cachedTransform_.localPosition = Vector3.zero;
			SetMeshDirty( );
		}

		// Called from Awake(), override in derived classes for additional functionality
		protected virtual void PostAwake( ) { }

		public void SetDepth( float d )
		{
			depth_ = d;
			SetMeshDirty( );
		}

		#endregion Setup

		#region Mesh

		// Creates the mesh if it doesn't yet exist
		protected Mesh GetMesh( bool remake )
		{
			Mesh mesh = cachedMeshFilter_.sharedMesh;
			if (mesh == null || remake)
			{
				cachedMeshFilter_.sharedMesh = new Mesh( );
				mesh = cachedMeshFilter_.sharedMesh;
			}
			return mesh;
		}

		protected Mesh GetMesh()
		{
			return GetMesh( false );
		}

		protected Mesh RecreateMesh()
		{
			return GetMesh( true );
		}

		abstract protected void DoAdjustMesh( );

		abstract public Vector3 DefaultPulseCentre( );

		#endregion

		#region Creation

		virtual public T Clone< T >( string name, float d ) where T : ElementBase
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
				Debug.Log( "Cloned  " + this.GetType( ).ToString( ) + " '" + gameObject.name + "' as a " + typeof( T ).ToString( ) + " called '" + name + "'" );
			}
			// Mesh may not have been created yet, in which case we can leave it to the clone to handle
			if (cachedMeshFilter.sharedMesh != null)
			{
				component.cachedMeshFilter.mesh = Mesh.Instantiate( cachedMeshFilter.sharedMesh );
			}
			component.SetField( field_ );
			component.SetDepth( d );
			component.OnClone( this );
			return component;
		}

		public T Clone< T >( string name ) where T : ElementBase
		{
			return Clone< T >( name, depth_ );
		}

		// Note - have done it this way tp leave possibility of cloning from other types (eg Quadrilateral from parallelogram from square, etc)
		protected abstract void OnClone< T >( T src ) where T : ElementBase;

		abstract public ElementBase Clone( string name );

		#endregion Creation
	}

}
