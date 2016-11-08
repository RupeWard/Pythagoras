using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Stores a list of elements by name, with access by name or name & type.
	Can be validated against an ElementListDefinition
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
		private string name_ = "[UNNAMED ELEMENT LIST]";

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

		public ElementList(string n, ElementList other )
		{
			name_ = n;
			CopyElementsFrom( other );
		}

		public ElementList( ElementList other)
		{
			name_ = other.name_ + " (CLONE)";
			CopyElementsFrom( other );
		}

		private void CopyElementsFrom( ElementList other )
		{
			foreach (string n in other.elements_.Keys)
			{
				elements_.Add( n, other.elements_[n] );
			}
		}

		#endregion Set-up

		#region modifiers

		/* Add an element to the list

			n must be non-empty
			e must be non-null
			list must not already contain an element called n

			returns the element added (or null)
		*/
		public ElementBase AddElement(string n, ElementBase e)
		{
			ElementBase result = null;
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
				result = e;
				if (DEBUG_ELEMENTLIST)
				{
					Debug.Log( "Added '" + n + "', now " + this.DebugDescribe( ) );
				}
			}
			return result;
		}

		/* Remove element called n from the list if it exists 
		
			destroy = also destroy the GO 

			returns true if element was found and removed
		*/
		public bool RemoveElement( string n, bool destroy )
		{
			bool success = false;
			if (elements_.ContainsKey(n))
			{
				if (DEBUG_ELEMENTLIST)
				{
					Debug.LogWarning( "Removing element '" + n + "' from " + this.DebugDescribe( ) );
				}
				if (destroy)
				{
					GameObject.Destroy( elements_[n].gameObject );
				}
				elements_.Remove( n );
				success = true;
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

		// Helper - remove without destroying
		public bool RemoveElement( string n)
		{
			return RemoveElement( n, false );
		}

		// Helper - remove and destroy
		public bool DestroyElement( string n )
		{
			return RemoveElement( n, true);
		}

		/* Remove element called n from the list if it exists and is of given type
		
			destroy = also destroy the GO 

			returns true if element was found and removed
		*/
		public bool RemoveElementOfType< T >( string n, bool destroy ) where T : ElementBase
		{
			if (DEBUG_ELEMENTLIST)
			{
//				Debug.LogWarning( "RemoveElementOfType< " + typeof( T ).ToString( ) + "( '" + n + "', " + destroy + " )" );
			}

			bool success = false;
			if (elements_.ContainsKey( n ))
			{
				T elementT = elements_[n] as T;
				if (elementT == null)
				{
					Debug.LogWarning( "Element '" + n + "' to remove is a "+elements_[n].GetType() +" not a "+typeof( T ).ToString()+ " in " + this.DebugDescribe( ) );
				}
				else
				{
					if (DEBUG_ELEMENTLIST)
					{
						Debug.LogWarning( "Removing element '" + n + "' from " + this.DebugDescribe( ) );
					}
					if (destroy)
					{
						GameObject.Destroy( elements_[n].gameObject );
					}
					elements_.Remove( n );
					success = true;
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

		// Helper - remove without destroying
		public bool RemoveElementOfType<T>( string n ) where T : ElementBase
		{
			return RemoveElementOfType<T>( n, false );
		}

		// Helper - remove and destroy
		public bool DestroyElementOfType<T>( string n ) where T : ElementBase
		{
			return RemoveElementOfType<T>( n, true );
		}

		/* Remove all elements

			destroy - also destroy them
		*/
		public void RemoveAllElements( bool destroy )
		{
			if (destroy)
			{
				foreach (KeyValuePair<string, ElementBase> kvp in elements_)
				{
					GameObject.Destroy( kvp.Value.gameObject );
				}
			}
			elements_.Clear( );
		}

		// Helper - remove all without destroying
		public void RemoveAllElements()
		{
			RemoveAllElements( false );
		}

		// Helper - remove all and destroy
		public void DestroyAllElements( )
		{
			RemoveAllElements( true );
		}

		/* Remove specific element the list if it exists 

			destroy = also destroy the GO 

			returns true if element was found and removed
		*/
		public bool RemoveElement( ElementBase eb, bool destroy )
		{
			bool found = false;
			string key = string.Empty;
			if (eb == null)
			{
				if (DEBUG_ELEMENTLIST)
				{
					Debug.LogWarning( "Null Element passed to RemoveElement in ElementList '" + name + "'" );
				}
			}
			else
			{
				foreach (KeyValuePair<string, ElementBase> kvp in elements_)
				{
					if (kvp.Value == eb)
					{
						key = kvp.Key;
						found = true;
						break;
					}
				}
				if (found)
				{
					if (destroy)
					{
						GameObject.Destroy( elements_[key].gameObject );
					}
					elements_.Remove( key );
				}
				else
				{
					if (DEBUG_ELEMENTLIST)
					{
						Debug.LogWarning( "Element '" + eb.name + "' not in ElementList '" + name + "' on trying to Remove" );
					}
				}
			}
			return found;
		}

		// Helper - rmeove without destroying
		public bool RemoveElement( ElementBase eb)
		{
			return RemoveElement( eb, false );
		}

		// Helper - rmeove and destroy
		public bool DestroyElement( ElementBase eb )
		{
			return RemoveElement( eb, true );
		}

		#endregion modifiers

		#region accessors

		/* Retrieve element by name

			required -> throw exception if true and element not found

			returns element or null if not found
		*/
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

		// Helper - retrieve by name but don't worry if not found (client must deal with it)
		public ElementBase GetElement(string n)
		{
			return GetElement( n, false );
		}

		// Helper - retrieve by name and throw exception if not found
		public ElementBase GetRequiredElement( string n )
		{
			return GetElement( n, true );
		}

		/* Pop: Retrieve element by name and also remove from list

			As RemoveElement(n, rrquired) but with renoval
		*/
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

		/* Retrieve element by name, if of type

			as non-generic version above but with type-checking
		*/
		public T GetElementOfType< T > ( string n, bool required ) where T : ElementBase
		{
			T result = null;
			ElementBase element;
			if (false == elements_.TryGetValue( n, out element ))
			{
				if (required)
				{
					throw new System.Exception( "ElementList '" + name_ + "' does not contain required element called '" + n + "'" );
				}
			}
			else
			{
				result = element as T;
				if (result == null)
				{
					Debug.LogError( "ElementList '" + name_ + "' contains an element called '" + n + "' but it is a " + element.GetType( ).ToString( ) + " rather than a " + typeof( T ).ToString( ) );
					if (required)
					{
						throw new System.Exception( "ElementList '" + name_ + "' contains an element called '" + n + "' but it is a " + element.GetType( ).ToString( ) + " rather than a " + typeof( T ).ToString( ) );
					}
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
					sb.Append( " ( '" ).Append( kvp.Key ).Append("', ");
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
