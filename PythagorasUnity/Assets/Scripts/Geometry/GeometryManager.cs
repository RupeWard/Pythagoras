﻿using UnityEngine;
using System.Collections;

/*
	Handles creation & placement of elements in fields
*/
public class GeometryManager : MonoBehaviour
{
	public bool testMode = true; // set to true to create some elements to play around with for testing

	#region prefabs

	public GameObject trianglePrefab;
	public GameObject parallelogramPrefab;

	#endregion prefabs

	#region private data

	private Field mainField_ = null; // currently only using the one Field

	#endregion private data

	#region Flow

	private void Awake()
	{
		mainField_ = new GameObject( "MainField" ).AddComponent<Field>( );
	}

	private void Start ()
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
				},
				Color.blue
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
				90f,
				Color.green
				);
		}
	}
	
	#endregion Flow

	#region creating elements

	// Instantiate a Triangle in a Field and set it up
	private Triangle AddTriangleToField( string n, Field f, float d, Vector2[] v, Color c)
	{
		GameObject triangleGO = GameObject.Instantiate<GameObject>( trianglePrefab ) as GameObject;
		triangleGO.name = n;
		Triangle triangle = triangleGO.GetComponent<Triangle>( );
		triangle.Init( f, d, v, c );
		return triangle;
	}

	// Instantiate a Parallelogram in a Field and set it up
	private Parallelogram AddParallelogramToField( string n, Field f, float d, Vector2[] bl, float h, float a, Color c )
	{
		GameObject parallelogramGO = GameObject.Instantiate<GameObject>( parallelogramPrefab ) as GameObject;
		parallelogramGO.name = n;
		Parallelogram parallelogram = parallelogramGO.GetComponent<Parallelogram>( );
		parallelogram.Init( f, d, bl, h, a, c );
		return parallelogram;
	}

	#endregion creating elements

}
