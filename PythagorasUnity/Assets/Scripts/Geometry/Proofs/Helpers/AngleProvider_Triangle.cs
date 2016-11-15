﻿using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	/*
	Get angle from triangle

	?? DEPRECATE ?? Currently using Polygon version instead
					Unlike e.g. parallelogram, triangle element is defined in terms of points not angle, so needs to compute angle on request. 
					So polygon one is PROBABLY more efficient (polygon angle element knows its angle)
					
		*** Check this efficiency theory before finalising *** 
	*/
	public class AngleProvider_Triangle : IAngleProvider
	{
		#region private data

		private string triangleName_ = "[UNKNOWN TRIANGLE]";
		private int angleNumber_ = 0;
		private GeometryHelpers.EAngleModifier eModifier_ = GeometryHelpers.EAngleModifier.Raw;

		#endregion

		#region setup

		public AngleProvider_Triangle( 
			string tn,
			int an,
			GeometryHelpers.EAngleModifier eam)
		{
			triangleName_ = tn;
			angleNumber_ = an;
			eModifier_ = eam;
		}

		#endregion setup

		#region IAngleProvider

		public float GetAngle( ElementList elements )
		{
			float result = 0f;

			Element_Triangle triangle = elements.GetRequiredElementOfType< Element_Triangle >( triangleName_ );
			float rawAngle = triangle.GetInternalAngleDegrees( angleNumber_ );
			result = GeometryHelpers.ModifyAngleDegrees( rawAngle, eModifier_ );
			return result;
		}

		#endregion
	}
}

