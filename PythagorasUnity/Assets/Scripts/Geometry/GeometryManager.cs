using UnityEngine;
using System.Collections;

public class GeometryManager : MonoBehaviour
{
	public bool testMode = true;

	#region prefabs

	public GameObject trianglePrefab;

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
				new Vector2[]
				{
				new Vector2(-1f, -1f),
				new Vector2(1f, -1f),
				new Vector2(1f, 1f)
				},
				0f );
		}
	}
	
	void Update ()
	{
	
	}

	#endregion Flow

	#region creating elements

	private Triangle AddTriangleToField( string n, Field f, Vector2[] v, float d)
	{
		GameObject triangleGO = GameObject.Instantiate<GameObject>( trianglePrefab ) as GameObject;
		triangleGO.name = n;
		Triangle triangle = triangleGO.GetComponent<Triangle>( );
		triangle.Init( f, v, d );
		return triangle;
	}

	#endregion creating elements

}
