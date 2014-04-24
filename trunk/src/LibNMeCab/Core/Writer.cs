//  MeCab -- Yet Another Part-of-Speech and Morphological Analyzer
//
//  Copyright(C) 2001-2006 Taku Kudo <taku@chasen.org>
//  Copyright(C) 2004-2006 Nippon Telegraph and Telephone Corporation
using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Core
{
    public class Writer
    {
        private const string FloatFormat = "f6";

        private delegate void WriteAction(StringBuilder os, MeCabNode bosNode);
        private WriteAction write;

        private string outputFormatType;

        public string OutputFormatType
        {
            get
            {
                return this.outputFormatType;
            }
            set
            {
                this.outputFormatType = value;
                switch (value)
                {
                    case "lattice":
                        this.write = this.WriteLattice;
                        break;
                    case "wakati":
                        this.write = this.WriteWakati;
                        break;
                    case "none":
                        this.write = this.WriteNone;
                        break;
                    case "dump":
                        this.write = this.WriteDump;
                        break;
                    case "em":
                        this.write = this.WriteEM;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(value + " is not supported Format");
                }
            }
        }

        public void Open(MeCabParam param)
        {
            this.OutputFormatType = param.OutputFormatType;
        }

        public void Write(StringBuilder os, MeCabNode bosNode)
        {
            this.write(os, bosNode);
        }

        public void WriteLattice(StringBuilder os, MeCabNode bosNode)
        {
            for (MeCabNode node = bosNode.Next; node.Next != null; node = node.Next)
            {
                os.Append(node.Surface);
                os.Append("\t");
                os.Append(node.Feature);
                os.AppendLine();
            }
            os.AppendLine("EOS");
        }

        public void WriteWakati(StringBuilder os, MeCabNode bosNode)
        {
            MeCabNode node = bosNode.Next;
            if (node.Next != null)
            {
                os.Append(node.Surface);
                for (node = node.Next; node.Next != null; node = node.Next)
                {
                    os.Append(" ");
                    os.Append(node.Surface);
                }
            }
            os.AppendLine();
        }

        public void WriteNone(StringBuilder os, MeCabNode bosNode)
        {
            // do nothing
        }

        public void WriteUser(StringBuilder os, MeCabNode bosNode)
        {
            throw new NotImplementedException();
        }

        public void WriteEM(StringBuilder os, MeCabNode bosNode)
        {
            const float MinProb = 0.0001f;
            for (MeCabNode node = bosNode; node != null; node = node.Next)
            {
                if (node.Prob >= MinProb)
                {
                    os.Append("U\t");
                    if (node.Stat == MeCabNodeStat.Bos)
                        os.Append("BOS");
                    else if (node.Stat == MeCabNodeStat.Eos)
                        os.Append("EOS");
                    else
                        os.Append(node.Surface);
                    os.Append("\t").Append(node.Feature);
                    os.Append("\t").Append(node.Prob.ToString(FloatFormat));
                    os.AppendLine();
                }
                for (MeCabPath path = node.LPath; path != null; path = path.LNext)
                {
                    if (path.Prob >= MinProb)
                    {
                        os.Append("B\t").Append(path.LNode.Feature);
                        os.Append("\t").Append(node.Feature);
                        os.Append("\t").Append(path.Prob.ToString(FloatFormat));
                        os.AppendLine();
                    }
                }
            }
            os.AppendLine("EOS");
        }

        public void WriteDump(StringBuilder os, MeCabNode bosNode)
        {
            for (MeCabNode node = bosNode; node != null; node = node.Next)
            {
#if NeedId
                os.Append(node.Id).Append(" ");
#endif
                if (node.Stat == MeCabNodeStat.Bos)
                    os.Append("BOS");
                else if (node.Stat == MeCabNodeStat.Eos)
                    os.Append("EOS");
                else
                    os.Append(node.Surface);

                os.Append(" ").Append(node.Feature);
                os.Append(" ").Append(node.BPos);
                os.Append(" ").Append(node.EPos);
                os.Append(" ").Append(node.RCAttr);
                os.Append(" ").Append(node.LCAttr);
                os.Append(" ").Append(node.PosId);
                os.Append(" ").Append(node.CharType);
                os.Append(" ").Append((int)node.Stat);
                os.Append(" ").Append(node.IsBest ? "1" : "0");
                os.Append(" ").Append(node.Alpha.ToString(FloatFormat));
                os.Append(" ").Append(node.Beta.ToString(FloatFormat));
                os.Append(" ").Append(node.Prob.ToString(FloatFormat));
                os.Append(" ").Append(node.Cost);

                for (MeCabPath path = node.LPath; path != null; path = path.LNext)
                {
#if NeedId
                    os.Append(" ").Append(path.LNode.Id);
#endif
                    os.Append(" ");
                    os.Append(":").Append(path.Cost);
                    os.Append(":").Append(path.Prob.ToString(FloatFormat));
                }

                os.AppendLine();
            }
        }

        public unsafe void WriteNode(StringBuilder os, char* p, string sentence, MeCabNode node)
        {
            for (; *p != 0x0; p++)
            {
                switch (*p)
                {
                    default: os.Append(*p); break;
                    case '%':
                        switch (*++p)
                        {
                            default: os.Append("unkonwn meta char ").Append(*p); break;
                            case 'S': os.Append(sentence); break;
                            case 'L': os.Append(sentence.Length); break;
                            case 'm': os.Append(node.Surface); break;
                            case 'M': os.Append(sentence, (node.BPos - node.RLength + node.Length), node.RLength); break;
                            case 'h': os.Append(node.PosId); break;
                            case '%': os.Append('%'); break;
                            case 'c': os.Append(node.WCost); break;
                            case 'H': os.Append(node.Feature); break;
                            case 't': os.Append(node.CharType); break;
                            case 's': os.Append(node.Stat); break;
                            case 'P': os.Append(node.Prob); break;
                            case 'p':
                                switch (*++p)
                                {
                                    default: throw new ArgumentException("[iseSCwcnblLh] is required after %p");
#if NeedId
                                    case 'i': os.Append(node.Id); break;
#else
                                    case 'i': throw new ArgumentException("%pi is not supported");
#endif
                                    case 'S': os.Append(sentence, node.BPos, (node.RLength - node.Length)); break;
                                    case 's': os.Append(node.BPos); break;
                                    case 'e': os.Append(node.EPos); break;
                                    case 'C': os.Append(node.Cost - node.Prev.Cost - node.WCost); break;
                                    case 'w': os.Append(node.WCost); break;
                                    case 'c': os.Append(node.Cost); break;
                                    case 'n': os.Append(node.Cost - node.Prev.Cost); break;
                                    case 'b': os.Append(node.IsBest ? '*' : ' '); break;
                                    case 'P': os.Append(node.Prob); break;
                                    case 'A': os.Append(node.Alpha); break;
                                    case 'B': os.Append(node.Beta); break;
                                    case 'l': os.Append(node.Length); break;
                                    case 'L': os.Append(node.RLength); break;
                                    case 'h':
                                        switch (*++p)
                                        {
                                            default: throw new ArgumentException("lr is required after %ph");
                                            case 'l': os.Append(node.LCAttr); break;
                                            case 'r': os.Append(node.RCAttr); break;
                                        }; break;
                                    case 'p':
                                        char mode = *++p;
                                        char sep = *++p;
                                        if (sep == '\\') sep = this.GetEscapedChar(*++p);
                                        if (node.LPath == null) throw new InvalidOperationException("no path information, use -l option");
                                        for (MeCabPath path = node.LPath; path != null; path = path.LNext)
                                        {
                                            if (path != node.LPath) os.Append(sep);
                                            switch (mode)
                                            {
                                                case 'i': os.Append(path.LNode.PosId); break;
                                                case 'c': os.Append(path.Cost); break;
                                                case 'P': os.Append(path.Prob); break;
                                                default: throw new ArgumentException("[icP] is required after %pp");
                                            }
                                        }
                                        break;
                                } break;
                            case 'f':
                            case 'F':
                                char separator = '\t';
                                if (*p == 'F')
                                    if (*++p == '\\')
                                        separator = this.GetEscapedChar(*++p);
                                    else
                                        separator = *p;
                                if (*++p != '[') throw new ArgumentException("cannot find '['");
                                string[] features = node.Feature.Split(',');
                                int n = 0;
                                while (true)
                                {
                                    if (char.IsDigit(*++p)) { n = n * 10 + (*p - '0'); continue; }
                                    if (n >= features.Length) throw new ArgumentException("given index is out of range");
                                    os.Append(features[n]);
                                    if (*++p == ',') { os.Append(separator); n = 0; continue; }
                                    if (*p == ']') break;
                                    throw new ArgumentException("cannot find ']'");
                                } 
                                break;
                        } break;
                }
            }
        }

        private char GetEscapedChar(char p)
        {
            switch (p)
            {
                case '0': return '\0';
                case 'a': return '\a';
                case 'b': return '\b';
                case 't': return '\t';
                case 'n': return '\n';
                case 'v': return '\v';
                case 'f': return '\f';
                case 'r': return '\r';
                case 's': return ' ';
                case '\\': return '\\';
                default: return '\0'; //never be here
            }
        }
    }
}
