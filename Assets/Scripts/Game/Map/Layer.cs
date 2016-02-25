// Layers definition
public class Layer
{
    public const int FloorNum           = 8;
	public const int TankNum 		    = 9;
	public const int BreakableNum 	    = 10;
	public const int UnbreakableNum 	= 11;
    
    public const int Floor              = (1 << FloorNum);
	public const int Tank 			    = (1 << TankNum);
	public const int Breakable 		    = (1 << BreakableNum);
	public const int Unbreakable 		= (1 << UnbreakableNum);
}
