using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class AngleProvider_Polygon : IAngleProvider
	{
		#region private data

		private string polygonName_ = "[UNKNOWN POLYGON]";
		private int angleNumber_ = 0;
		private GeometryHelpers.EAngleModifier eModifier_ = GeometryHelpers.EAngleModifier.Raw;

		#endregion

		#region setup

		public AngleProvider_Polygon( 
			string tn,
			int an,
			GeometryHelpers.EAngleModifier eam)
		{
			polygonName_ = tn;
			angleNumber_ = an;
			eModifier_ = eam;
		}

		#endregion setup

		#region IAngleProvider

		public float GetAngle( ElementList elements )
		{
			float result = 0f;

			ElementPolygonBase polygon = elements.GetRequiredElementOfType< ElementPolygonBase >( polygonName_);
			Element_Sector sector = polygon.GetAngleElement( angleNumber_ );
			if (sector != null)
			{
				float rawAngle = sector.angleExtentDegrees;
				result = GeometryHelpers.ModifyAngleDegrees( rawAngle, eModifier_ );
			}
			else
			{
				Debug.LogError( "Polygon " + polygonName_ + " has no angleElement " + angleNumber_ );
			}
			return result;
		}

		#endregion
	}
}

