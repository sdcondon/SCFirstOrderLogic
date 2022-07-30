using BenchmarkDotNet.Running;
using System.Reflection;

namespace SCPropositionalLogic
{
    public class Program
    {
        /// <summary>
        /// Application entry point. See https://benchmarkdotnet.org/articles/guides/console-args.html or run app with --help for 
        /// available command line parameters. ..Or just run without arguments and follow the prompts.
        /// </summary>
        public static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
    }
}