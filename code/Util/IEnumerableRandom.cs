using System;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableRandom
{
	public static T GetRandom<T>( this IEnumerable<T> e )
	{
		var count = e.Count();
		var rand = new Random().Next( count );
		var i = 0;
		foreach ( var itm in e )
			if ( i == rand ) return itm;
			else i++;

		return default;
	}
}
