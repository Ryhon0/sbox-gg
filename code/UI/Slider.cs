using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public class Slider : Panel
{
	public Panel Bar;
	public Panel BarFill;
	public Panel Handle;
	public TextEntry Entry;

	public float Value = 0.5f;

	public Slider()
	{
		Bar = Add.Panel( "bar" );
		BarFill = Bar.Add.Panel( "fill" );
		Bar.AddEventListener( "OnMouseDown", ( e ) =>
		{
			Dragging = true;
		} );
		Bar.AddEventListener( "OnMouseUp", ( e ) =>
		{
			Dragging = false;
		} );

		Handle = Bar.Add.Panel( "handle" );

		Entry = Add.TextEntry( null );
		Entry.AddClass( "entry" );
	}

	bool Dragging;

	[Event.Frame]
	void Draw()
	{
		UpdateValue();
	}

	public virtual void UpdateValue()
	{
		if ( Dragging )
		{
			var mouse = Bar.MousePosition;
			var w = Bar.Box.Rect.width;
			Value = Math.Clamp( mouse.x / w, 0, 1 );
		}

		BarFill.Style.Width = Length.Fraction( Value );
		Handle.Style.Left = Length.Fraction( Value );

		BarFill.Style.Dirty();
		Handle.Style.Dirty();
	}
}

public class ConvarSlider : Slider
{
	public string Convar;
	public float MinValue;
	public float MaxValue;

	public ConvarSlider( string convar, float min, float max ) : base()
	{
		Convar = convar;
		MinValue = min;
		MaxValue = max;
	}

	public override void UpdateValue()
	{
		Value = Math.Clamp( (float.Parse( ConsoleSystem.GetValue( Convar ) ) - MinValue) / (MaxValue - MinValue), 0, 1 );

		base.UpdateValue();

		ConsoleSystem.Run( Convar, (Value * MaxValue) + MinValue );
	}
}
