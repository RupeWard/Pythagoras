using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class AngleProvider_Polygon : IAngleProvider
	{
		#region private data

		private IElementProvider polygonProvider_ = null;
		private int angleNumber_ = 0;
		private GeometryHelpers.EAngleModifier eModifier_ = GeometryHelpers.EAngleModifier.Raw;

		#endregion

		#region setup

		public AngleProvider_Polygon( 
			string pn,
			int an,
			GeometryHelpers.EAngleModifier eam)
		{
			polygonProvider_ = new ElementProvider_Name( pn );
			angleNumber_ = an;
			eModifier_ = eam;
		}

		#endregion setup

		#region IAngleProvider

		public float GetAngle( ElementList elements )
		{
			float result = 0f;

			ElementPolygonBase polygon = polygonProvider_.GetElement< ElementPolygonBase >( elements );
			Element_Sector sector = polygon.GetAngleElement( angleNumber_ );
			if (sector != null)
			{
				float rawAngle = sector.angleExtentDegrees;
				result = GeometryHelpers.ModifyAngleDegrees( rawAngle, eModifier_ );
			}
			else
			{
				throw new System.Exception( "Polygon " + polygon.name+ " has no angleElement " + angleNumber_ );
			}
			return result;
		}

		#endregion
	}
}

