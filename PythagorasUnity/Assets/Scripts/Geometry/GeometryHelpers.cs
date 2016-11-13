using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public static class GeometryHelpers 
	{
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
		public static float ModifyAngle(float rawAngle, EAngleModifier eModifier)
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
	}
}
