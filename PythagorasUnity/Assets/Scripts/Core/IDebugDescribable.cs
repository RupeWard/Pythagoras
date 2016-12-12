using System;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Core
{
	public interface IDebugDescribable
	{
		void DebugDescribe( System.Text.StringBuilder sb );
	}
}

public static class DebugDescribeExtensions
{
	private static System.Text.StringBuilder sb = new System.Text.StringBuilder( );
	public static string DebugDescribe( this RJWard.Core.IDebugDescribable obj )
	{
		sb.Length = 0;
		obj.DebugDescribe( sb );
		return sb.ToString( );
	}

	public static void DebugDescribe( this System.Text.StringBuilder sb, RJWard.Core.IDebugDescribable dd )
	{
		sb.Append( (dd == null) ? ("NULL") : (dd.DebugDescribe( )) );
	}

	public static void DebugDescribe< T >(this IEnumerable<T> coll, System.Text.StringBuilder sb)
	{
		sb.Append( "(" );
		bool first = true;
		foreach (T t in coll)
		{
			if (first)
			{
				first = false;
			}
			else
			{
				sb.Append( ", " );
			}
			sb.Append( t.ToString( ) );			
		}
		sb.Append( ")" );
	}
}

