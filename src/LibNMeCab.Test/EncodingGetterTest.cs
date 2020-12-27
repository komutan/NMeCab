using NMeCab.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LibNMeCab.Test
{
    public class EncodingGetterTest
    {
        public static object[][] GetEncodingSource = new object[][]
        {
            // net coreでは、Unicode系以外が切り捨てられたか標準サポートされるエンコードが極端に少なくなり、
            // 実際的に使えるのはutf-8のみ
            new object[]{ Encoding.UTF8, "utf-8" },
            new object[]{ Encoding.UTF8, "UTF-8" },
            new object[]{ Encoding.UTF8, "utf8" },
            new object[]{ Encoding.UTF8, "UTF8" },
            new object[]{ Encoding.UTF8, "utf_8" },
            new object[]{ Encoding.UTF8, "UTF_8" },
            new object[]{ null, "xxx" },
            new object[]{ null, "" },
            new object[]{ null, "-" },
            new object[]{ null, null },
        };

        [Theory]
        [MemberData(nameof(GetEncodingSource))]
        public void GetEncoding(Encoding expected, string input)
        {
            Assert.Equal(expected, input.GetEncodingOrNull());
        }
    }
}
