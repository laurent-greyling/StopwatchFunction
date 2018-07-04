using CommandLine;

namespace TriggerStopwatch
{
    public class Options
    {
        [Option('u', "username", Required = true, HelpText = "User Name")]
        public string UserName { get; set; }

        [Option('s', "stopwatchname", Required = true, HelpText = "Stopwatch Name")]
        public string StopwatchName { get; set; }

    }
}
