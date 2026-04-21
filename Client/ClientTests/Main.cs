using Virtava.Client;

public static class Program
{
    public static void Main()
    {
        var client = new TrackingServerListener(14210, 14211);
    }
}