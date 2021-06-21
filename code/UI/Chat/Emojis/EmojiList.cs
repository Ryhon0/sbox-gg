using Sandbox;
using System.Collections.Generic;
using System.Text.Json;
public class Category
{
	public string Name { get; set; }
	public List<Emoji> Emojis { get; set; } = new();
}

public class Emoji
{
	public List<string> Names { get; set; }
	public string Unicode { get; set; }
}

public static class Emojis
{
	public static List<Category> Categories
	{
		get
		{
			if ( categories == null )
				categories = JsonSerializer.Deserialize<List<Category>>
					( FileSystem.Mounted.ReadAllText( "config/emojis.json" ) );

			return categories;
		}
	}
	static List<Category> categories;
}
