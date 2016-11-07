using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Stores a list of elements 
*/
namespace RJWard.Geometry
{
	public class ElementList : RJWard.Core.IDebugDescribable
	{
#if UNITY_EDITOR
		static private bool THROW_EXCEPTIONS = true;
#else
		static private bool THROW_EXCEPTIONS = false;
#endif

		static public bool DEBUG_ELEMENTLIST = true;

		#region private data

		private Dictionary<string, ElementBase> elements_ = new Dictionary<string, ElementBase>( );
		private string name_ = "[UN-NAMED ELEMENT LIST]";

		#endregion private data

		#region properties

		public int NumElements
		{
			get { return elements_.Count;  }
		}

		public string name
		{
			get { return name_;  }
		}

		#endregion properties

		#region Set-up

		public ElementList( string n )
		{
			name_ = n;
		}

		public ElementList( ElementList other)
		{
			name_ = other.name_ + "_CLONE";
			foreach ( string n in other.elements_.Keys)
			{
				elements_.Add( n, elements_[n] );
			}
		}

		#endregion Set-up

		#region modifiers

		public bool AddElement(string n, ElementBase e)
		{
			bool success = false;

			if (n.Length == 0)
			{
				Debug.LogError( "Name is empty on trying to add to ElementList '"+name_+"'" );
				if (THROW_EXCEPTIONS)
				{
					throw new System.ArgumentNullException( "Name is empty on trying to add to ElementList '"+name_+"'" );
				}
			}
			else if (e == null)
			{
				Debug.LogError( "Element is null on trying to add to ElementList '"+name_+"'" );
				if (THROW_EXCEPTIONS)
				{
					throw new System.ArgumentNullException( "Element is null on trying to add to ElementList '" + name_ + "'" );
				}
			}
			else if (elements_.ContainsKey( n ))
			{
				Debug.LogError( "ElementList '" + name_ + "' already contains an element with name '" + n + "' of type " + elements_[n].GetType( ).ToString( ) + " on trying to add another of type " + e.GetType( ).ToString( ) );
				if (THROW_EXCEPTIONS)
				{
					throw new System.Exception( "ElementList '" + name_ + "' already contains an element with name '" + n + "' of type " + elements_[n].GetType( ).ToString( ) + " on trying to add another of type " + e.GetType( ).ToString( ) );
				}
			}
			else
			{
				elements_.Add( n, e );
				if (DEBUG_ELEMENTLIST)
				{
					Debug.Log( "Added '" + n + "', now " + this.DebugDescribe( ) );
				}
			}

			return success;
		}

		public bool RemoveElement( string n )
		{
			bool success = false;
			if (elements_.ContainsKey(n))
			{
				if (DEBUG_ELEMENTLIST)
				{
					Debug.LogWarning( "Removing element '" + n + "' from " + this.DebugDescribe( ) );
				}
				elements_.Remove( n );
			}
			else
			{
				if (DEBUG_ELEMENTLIST)
				{
					Debug.LogWarning( "No element '" + n + "' to remove in " + this.DebugDescribe( ) );
				}
			}
			return success;
		}

		public bool RemoveElementOfType< T >( string n ) where T : ElementBase
		{
			bool success = false;
			if (elements_.ContainsKey( n ))
			{
				if (elements_[n] is T)
				{
					Debug.LogWarning( "Element '" + n + "' to remove is not a "+typeof( T ).ToString()+ " in " + this.DebugDescribe( ) );
				}
				else
				{
					if (DEBUG_ELEMENTLIST)
					{
						Debug.LogWarning( "Removing element '" + n + "' from " + this.DebugDescribe( ) );
					}
					elements_.Remove( n );
				}
			}
			else
			{
				if (DEBUG_ELEMENTLIST)
				{
					Debug.LogWarning( "No element '" + n + "' to remove in " + this.DebugDescribe( ) );
				}
			}
			return success;
		}


		#endregion modifiers

		#region accessors

		public ElementBase GetElement (string n, bool required)
		{
			ElementBase result;
			if (false == elements_.TryGetValue( n, out result ))
			{
				if (required)
				{
					throw new System.Exception( "ElementList '" + name_ + "' does not contain required element called '" + n + "'" );
				}
			}
			return result;
		}

		public ElementBase GetElement(string n)
		{
			return GetElement( n, false );
		}

		public ElementBase GetRequiredElement( string n )
		{
			return GetElement( n, true );
		}

		public ElementBase PopElement( string n, bool required)
		{
			ElementBase e = GetElement( n, required );
			if (e != null)
			{
				elements_.Remove( n );
			}
			return e;
		}

		public ElementBase PopElement( string n)
		{
			return PopElement( n, false );
		}

		public ElementBase PopRequiredElement( string n )
		{
			return PopElement( n, true );
		}

		public T GetElementOfType< T > ( string n, bool required ) where T : ElementBase
		{
			ElementBase element;
			if (false == elements_.TryGetValue( n, out element ))
			{
				if (required)
				{
					throw new System.Exception( "ElementList '" + name_ + "' does not contain required element called '" + n + "'" );
				}
			}
			T result = element as T;
			if (result == null)
			{
				Debug.LogError( "ElementList '" + name_ + "' contains an element called '" + n + "' but it is a " + element.GetType( ).ToString( ) + " rather than a " + typeof( T ).ToString( ) );
				if (required)
				{
					throw new System.Exception( "ElementList '" + name_ + "' contains an element called '" + n + "' but it is a " + element.GetType( ).ToString( ) + " rather than a " + typeof( T ).ToString( ) );
				}
			}
			return result;
		}

		public T GetElementOfType<T>( string n ) where T : ElementBase
		{
			return GetElementOfType<T>( n, false );
		}

		public T GetRequiredElementOfType<T>( string n ) where T : ElementBase
		{
			return GetElementOfType<T>( n, true );
		}

		
		#endregion accessors

		#region IDebugDescribable

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "ElementList '" + name_ + "': " + elements_.Count + " elements " );
			if (elements_.Count > 0)
			{
				foreach( KeyValuePair< string, ElementBase > kvp in elements_)
				{
					sb.Append( " (" ).Append( kvp.Key ).Append(" ");
					if (kvp.Value == null)
					{
						sb.Append( "NULL!" );
					}
					else
					{
						sb.Append( kvp.Value.GetType( ).ToString( ) );
					}
					sb.Append( ")" );
				}
			}
		}

		#endregion IDebugDescribable
	}
}
