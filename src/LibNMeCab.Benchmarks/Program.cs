using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace LibNMeCab.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args, GetGlobalConfig());
        }

        public static IConfig GetGlobalConfig()
        {
            return DefaultConfig.Instance
                    .AddJob(Job.ShortRun);
        }
    }
}
