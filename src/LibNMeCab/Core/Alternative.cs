#if NET20 || NET30 || NET35 || NETSTANDARD1_3
#pragma warning disable CS1591
#endif

#if NET20
namespace NMeCab.Core
{
    public delegate T Func<T>();
}

namespace System.Runtime.CompilerServices
{

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    { }
}
#endif

#if NET20 || NET30 || NET35
namespace NMeCab.Core
{
    public static class AlternativeExtentions
    {
        public static void Clear(this System.Text.StringBuilder stb)
        {
            stb.Length = 0;
        }
    }
}
#endif

#if NETSTANDARD1_3
namespace NMeCab.Core
{
    public class AppDomain
    {
        public static AppDomain CurrentDomain { get; } = new AppDomain();

        public string BaseDirectory { get; } = "";

        private AppDomain() { }
    }
}
#endif