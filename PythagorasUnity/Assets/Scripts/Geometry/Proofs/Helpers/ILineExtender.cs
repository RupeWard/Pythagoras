using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public interface ILineExtender 
	{
		bool ExtendLine( ElementList elementList );
		bool ExtendLine( Element_StraightLine line );
	}
}
