//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MeCab.Core
{
    public class CharProperty
    {
        #region Const/Field/Property

        private const string CharPropertyFile = "char.bin";

        private string[] cList;

        private readonly CharInfo[] charInfoList = new CharInfo[0xFFFF];

        public int Size
        {
            get { return this.cList.Length; }
        }

        #endregion

        #region Open

        public void Open(string dicDir)
        {
            string fileName = Path.Combine(dicDir, CharPropertyFile);

            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                this.Open(reader, fileName);
            }
        }

        public void Open(BinaryReader reader, string fileName = null)
        {
            uint cSize = reader.ReadUInt32();

            if (reader.BaseStream.CanSeek)
            {
                long fSize = sizeof(uint) + 32 * cSize + sizeof(uint) * charInfoList.Length;
                if (reader.BaseStream.Length != fSize)
                    throw new MeCabInvalidFileException("invalid file size", fileName);
            }

            this.cList = new string[cSize];
            for (int i = 0; i < this.cList.Length; i++)
            {
                this.cList[i] = StrUtils.GetString(reader.ReadBytes(32), Encoding.ASCII);
            }

            for (int i = 0; i < this.charInfoList.Length; i++)
            {
                this.charInfoList[i] = new CharInfo(reader.ReadUInt32());
            }
        }

        #endregion

        #region Get Infometion

        public string Name(int i)
        {
            return this.cList[i];
        }

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

        public CharInfo GetCharInfo(char c)
        {
            return this.charInfoList[c];
        }

        #endregion
    }
}
