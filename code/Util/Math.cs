public static class MathX
{
	public static int Wrap( this int value, int min, int max )
	{
		return (value % max) == 0 ? min : value % max;
	}
}
