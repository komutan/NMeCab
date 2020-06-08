using NMeCab.Core;
using Xunit;

namespace LibNMeCabTest
{
    public class CsvParseTest
    {
        [Theory]
        [InlineData(@"a", new[] { "a" })]
        [InlineData(@"a,b", new[] { "a", "b" })]
        [InlineData(@"a,b,c", new[] { "a", "b", "c" })]
        [InlineData(@"a,b,c,d,e", new[] { "a", "b", "c", "d", "e" })]
        [InlineData(@"""a""", new[] { "a" })]
        [InlineData(@"""a"",""b""", new[] { "a", "b" })]
        [InlineData(@"""a"",""b"",""c""", new[] { "a", "b", "c" })]
        [InlineData(@"""a"",""b"",""c"",""d"",""e""", new[] { "a", "b", "c", "d", "e" })]
        [InlineData(@"""a,b"",c,d,e", new[] { "a,b", "c", "d", "e" })]
        [InlineData(@"a,""b,c,d"",e", new[] { "a", "b,c,d", "e" })]
        [InlineData(@"a,b,c,""d,e""", new[] { "a", "b", "c", "d,e" })]
        [InlineData(@"""a,b,c,d,e""", new[] { "a,b,c,d,e" })]
        [InlineData(@"""a."",""b."",""c.""", new[] { "a.", "b.", "c." })]
        [InlineData(@""".a"","".b"","".c""", new[] { ".a", ".b", ".c" })]
        [InlineData(@""""",b,c", new[] { @"""", "b", "c" })]
        [InlineData(@"a,"""",c", new[] { "a", @"""", "c" })]
        [InlineData(@"a,b,""""", new[] { "a", "b", @"""" })]
        [InlineData(@"""""a,b,c", new[] { @"""a", "b", "c" })]
        [InlineData(@"a"""",b,c", new[] { @"a""", "b", "c" })]
        [InlineData(@"a,""""b"""",c", new[] { "a", @"""b""", "c" })]
        [InlineData(@"a,b,""""c", new[] { "a", "b", @"""c" })]
        [InlineData(@"a,b,c""""", new[] { "a", "b", @"c""" })]
        public void SplitCsvRow(string input, string[] expected)
        {
            var actual = StrUtils.SplitCsvRow(input, 1, 1);
            Assert.Equal(expected, actual);
        }
    }
}
