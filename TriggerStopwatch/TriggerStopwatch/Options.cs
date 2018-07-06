using CommandLine;

namespace TriggerStopwatch
{
    public class Options
    {
        [Option('u', "username", Required = true, HelpText = "User Name")]
        public string UserName { get; set; }

        [Option('s', "stopwatchname", Required = true, HelpText = "Stopwatch Name")]
        public string StopwatchName { get; set; }

        [Option('b', "start", Required = false, HelpText = "Start Stopwatch")]
        public bool Start { get; set; }

        [Option('k', "stop", Required = false, HelpText = "Stop Stopwatch")]
        public bool Stop { get; set; }

        [Option('r', "restart", Required = false, HelpText = "Restart Stopwatch")]
        public bool Restart { get; set; }

        [Option('a', "reset", Required = false, HelpText = "Reste Stopwatch")]
        public bool Reset { get; set; }

    }
}
