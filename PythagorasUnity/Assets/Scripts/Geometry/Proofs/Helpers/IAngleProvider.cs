using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public interface IAngleProvider 
	{
		float GetAngle( ElementList elementList );
	}
}
