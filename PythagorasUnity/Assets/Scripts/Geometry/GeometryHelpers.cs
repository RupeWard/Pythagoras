using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public static class GeometryHelpers 
	{
		#region tags

		public const string Tag_SubElement = "SubElement";

		#endregion tags

		#region element layering

		static public float internalLayerSeparation = 0.01f; // use for distance between layers of the same element (eg a polygon and its sides)
		static public int maxNumInternalElementLayers = 5; // Need to increase this if one ends up using more such layers
		static public float externalLayerSeparation
		{
			get { return maxNumInternalElementLayers * internalLayerSeparation;  }
		}

		#endregion element layering

		#region angle modifers

		public enum EAngleModifier
		{
			Raw,			// x
			Complementary,	// 90 - x
			Supplementary,	// 180 - x
			Explementary,	// 360 - x
		}

		// Applies modifier to angle
		public static float ModifyAngleDegrees(float rawAngle, EAngleModifier eModifier)
		{
			float result = rawAngle;
			switch (eModifier)
			{
				case EAngleModifier.Raw:
					{
						result = rawAngle;
						break;
					}
				case EAngleModifier.Complementary:
					{
						result = 90f - rawAngle;
						break;
					}
				case EAngleModifier.Supplementary:
					{
						result = 180f - rawAngle;
						break;
					}
				case EAngleModifier.Explementary:
					{
						result = 360f - rawAngle;
						break;
					}
			}
			return result;
		}
		#endregion angle modifers

		#region computations

		static public bool LineIntersectionPoint( Vector2[] line1, Vector2[] line2, ref Vector2 intersection )
		{
			if (line1.Length != 2 || line2.Length != 2)
			{
				throw new System.ArgumentException( "Line vector arrays passed to LineIntersectionPoint are " + line1.Length + " and " + line2.Length );
			}

			// Get A,B,C of first line - points : ps1 to pe1
			float A1 = line1[1].y - line1[0].y;
			float B1 = line1[0].x - line1[1].x;
			float C1 = A1 * line1[0].x + B1 * line1[0].y;

			// Get A,B,C of second line - points : ps2 to pe2
			float A2 = line2[1].y - line2[0].y;
			float B2 = line2[0].x - line2[1].x;
			float C2 = A2 * line2[0].x + B2 * line2[0].y;

			// Get delta and check if the lines are parallel
			float delta = A1 * B2 - A2 * B1;
			if (delta == 0)
			{
				return false;
			}

			// now set the Vector2 intersection point and return true
			intersection.Set (
				(B2 * C1 - B1 * C2) / delta,
				(A1 * C2 - A2 * C1) / delta
			);

			return true;
		}

		#endregion computations
	}
}
