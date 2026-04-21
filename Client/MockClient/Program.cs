using Virtava.Client;

namespace MockClient;

class Program
{
    static void Main(string[] args)
    {
        var listener = new TrackingServerListener(14210, 14211);
        listener.OnResultReceived += (r) => Console.WriteLine($"  RESULTS ALERT! {r.TrackingSucceded}");
    }
}
