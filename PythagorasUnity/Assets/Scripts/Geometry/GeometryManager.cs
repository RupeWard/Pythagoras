using UnityEngine;
using System.Collections;

public class GeometryManager : MonoBehaviour
{
	public bool testMode = true;

	#region prefabs

	public GameObject trianglePrefab;
	public GameObject parallelogramPrefab;

	#endregion prefabs

	#region private data

	private Field mainField_ = null;

	#endregion private data

	#region Flow

	private void Awake()
	{
		mainField_ = new GameObject( "MainField" ).AddComponent<Field>( );
	}

	void Start ()
	{
		if (testMode)
		{
			AddTriangleToField(
				"TestTri",
				mainField_,
				0f,
				new Vector2[]
				{
				new Vector2(-1f, -2f),
				new Vector2(1f, -2f),
				new Vector2(1f, 0f)
				}
				);
			AddParallelogramToField(
				"TestPar",
				mainField_,
				0f,
				new Vector2[]
				{
				new Vector2(-1f, 1f),
				new Vector2(1f, 1f)
				},
				1f,
				90f
				);
		}
	}
	
	void Update ()
	{
	
	}

	#endregion Flow

	#region creating elements

	private Triangle AddTriangleToField( string n, Field f, float d, Vector2[] v)
	{
		GameObject triangleGO = GameObject.Instantiate<GameObject>( trianglePrefab ) as GameObject;
		triangleGO.name = n;
		Triangle triangle = triangleGO.GetComponent<Triangle>( );
		triangle.Init( f, d, v );
		return triangle;
	}

	private Parallelogram AddParallelogramToField( string n, Field f, float d, Vector2[] bl, float h, float a )
	{
		GameObject parallelogramGO = GameObject.Instantiate<GameObject>( parallelogramPrefab ) as GameObject;
		parallelogramGO.name = n;
		Parallelogram parallelogram = parallelogramGO.GetComponent<Parallelogram>( );
		parallelogram.Init( f, d, bl, h, a );
		return parallelogram;
	}


	#endregion creating elements

}
