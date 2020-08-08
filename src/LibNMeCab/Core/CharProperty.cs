//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation

#pragma warning disable CS1591

using System.IO;
using System.Runtime.CompilerServices;

namespace NMeCab.Core
{
    public class CharProperty
    {
        #region Const/Field/Property

        private const string CharPropertyFile = "char.bin";

        private byte[][] cList;

        private readonly CharInfo[] charInfoList = new CharInfo[0xFFFF];

        public int Size
        {
            get { return this.cList.Length; }
        }

        #endregion

        #region Open

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Open(string dicDir)
        {
            string fileName = Path.Combine(dicDir, CharPropertyFile);

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
                this.Open(reader, fileName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Open(BinaryReader reader, string fileName = null)
        {
            uint cSize = reader.ReadUInt32();

            if (reader.BaseStream.CanSeek)
            {
                long fSize = sizeof(uint) + 32 * cSize + sizeof(uint) * charInfoList.Length;
                if (reader.BaseStream.Length != fSize)
                    throw new InvalidDataException($"invalid file size. {fileName ?? ""}");
            }

            this.cList = new byte[cSize][];
            for (int i = 0; i < this.cList.Length; i++)
            {
                this.cList[i] = reader.ReadBytes(32);
            }

            for (int i = 0; i < this.charInfoList.Length; i++)
            {
                this.charInfoList[i] = new CharInfo(reader.ReadUInt32());
            }
        }

        #endregion

        #region Get Infometion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Name(int i)
        {
            return this.cList[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe char* SeekToOtherType(char* begin, char* end, CharInfo c, CharInfo* fail, int* cLen)
        {
            char* p = begin;
            *cLen = 0;

            *fail = this.GetCharInfo(*p);

            while (p != end && c.IsKindOf(*fail))
            {
                p++;
                (*cLen)++;
                c = *fail;

                *fail = this.GetCharInfo(*p);
            }

            return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CharInfo GetCharInfo(char c)
        {
            return this.charInfoList[c];
        }

        #endregion
    }
}
