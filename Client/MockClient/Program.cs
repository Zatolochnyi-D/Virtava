using Virtava.Client;

namespace MockClient;

class Program
{
    static void Main(string[] args)
    {
        var listener = new TrackingServerListener<TrackingResult>(14210, 14211);
        listener.OnResultReceived += (r) => Console.WriteLine($"  RESULTS ALERT! {r.TrackingSucceded}");
        // TODO: Make proper close and see how it plays out.
    }
}