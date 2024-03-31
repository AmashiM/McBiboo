using McBiboo;

public class Program
{
    public static void Main(string[] args)
    {
        Server server = new Server("C:\\Users\\Amash\\source\\repos\\McBiboo\\McBiboo\\server\\");

        server.PrintPaths();

        Console.WriteLine(server.BehaviorPacks.Length);
        Console.WriteLine(server.ResourcePacks.Length);

        //server.NewAddon("C:\\Users\\Amash\\source\\repos\\McBiboo\\McBiboo\\server\\addons\\utility_hud_by_ambient.mcpack");
        //server.NewAddon("C:\\Users\\Amash\\source\\repos\\McBiboo\\McBiboo\\server\\addons\\Tinkers Construct (v2.0).mcaddon");
        //server.NewAddon("C:\\Users\\Amash\\source\\repos\\McBiboo\\McBiboo\\server\\packs\\Tinkers Construct RP.zip");
        server.CycleNewAddons(server.AddonsPath);
        server.CycleNewAddons(server.PacksPath);
        server.Setup();
    }
}