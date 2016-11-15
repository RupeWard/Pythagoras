using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public interface IStraightLineProvider 
	{
		Element_StraightLine GetLine( ElementList elementList );
	}
}
