using Mesh.CNC_Turning.Code.Sinumerik;
using Simulation.CNC_Turning.Code;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simulation.CNC_Turning.Interpretation
{
    class Interpreter
    {
        string filepath;
        DocumentModel m_DocModel;
        internal Interpreter(string filepath, DocumentModel doc)
        {
            this.filepath = filepath;
            this.m_DocModel = doc;
        }

        internal NCProgram Interpret(out string[] errors)
        {
            
            List<string> lines = new List<string>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filepath);
            while ((line = file.ReadLine()) != null)
            {
                line = line.ToUpper();
                lines.Add(line);
            }

            file.Close();

            SinumerikProgram prog = new SinumerikProgram();
            string pattern = @"[A-Z\d.-]+";
            for (int i = 0; i < lines.Count; i++)
            {
                string l = lines[i];
                SearchComment(ref l);
                var matches = Regex.Matches(l, pattern);
                if(matches.Count == 0)
                    continue;          
                checkMatchesLine(prog, matches, i, prog);
            }
            CheckProgramEnd(prog.Sentences);
            CheckProgramSemantics(prog);
            errors = new string[Errors.Count + 1];
            errors[0] = Errors.Count + " error(s) detected";
            for (int i = 1; i <= Errors.Count; i++)
                errors[i] = Errors[i-1].ToString();
            IsErrorFree = Errors.Count == 0;
            return prog;
        }

        private void CheckProgramSemantics(SinumerikProgram prog)
        {
            short currentTool = -1;
            for (int i = 0; i < prog.Sentences.Count; i++)
            {
                Sentence s = prog.Sentences[i];
                if (s.Y < 0)
                    Errors.Add(new Error(i + 1, 0, "X coordinates must not be negative"));
                if (s.G[0] || s.G[1] || s.G[2] || s.G[3])
                {
                    if (currentTool < 0)
                        Errors.Add(new Error(i + 1, 0, "No tool was specified"));
                }
                if (s.T >= 0)
                    currentTool = s.T;
            }
        }

        private void CheckProgramEnd(List<Sentence> lines)
        {
            if(lines.Count != 0 && lines[lines.Count - 1].M[30] == false)
              Errors.Add(new Error(lines.Count, 0, "Illegal program end: M30 not found"));
        }

        private void SearchComment(ref string line)
        {
            int pos = 0;
            for (pos = 0; pos < line.Length; pos++)
            {
                if (line[pos] == ';')
                {
                    break;
                }

            }
            line = line.Substring(0, pos);
        }

        private void checkMatchesLine(NCProgram prog, MatchCollection matches, int line, SinumerikProgram p)
        {
            SinumerikSentence s = new SinumerikSentence();
            foreach (Match m in matches)
            {
                switch (m.Value[0])
                {
                    case 'N': CheckFourDigitWord(out s.NWord, m, line, m.Index, "N");  break;
                    case 'G': CheckTwoDigitWord(ref s.GWords, m, line, m.Index, "G"); break;
                    case 'X': CheckDecimalWord(out s.YWord, m, line, m.Index, "X");  break;
                    case 'Z': CheckDecimalWord(out s.XWord, m, line, m.Index, "Z"); break;
                    case 'I': CheckDecimalWord(out s.IWord, m, line, m.Index, "I"); break;
                    case 'K': CheckDecimalWord(out s.KWord, m, line, m.Index, "K"); break;
                    case 'F': CheckFourDigitWord(out s.FWord, m, line, m.Index, "F"); break;
                    case 'T': CheckFourDigitWord(out s.TWord, m, line, m.Index, "T"); CheckTool(s.TWord, line, m.Index);  break;
                    case 'S': CheckFourDigitWord(out s.SWord, m, line, m.Index, "S"); break;
                    case 'M': CheckTwoDigitWord(ref s.MWords, m, line, m.Index, "M"); break;
                    default: Errors.Add(new Error(line+1, 0, "Illegal sentence")); break;
                }
            }
            prog.AddSentence(s);
        }

        private void CheckTool(short toolNumber, int line, int col)
        {
            if(toolNumber < -1 || toolNumber > 9999)
                Errors.Add(new Error(line + 1, col, "Illegal tool number"));
            if(m_DocModel.ToolSet.GetSlotByName(toolNumber) == null)
                Errors.Add(new Error(line + 1, col, "Tool not found"));
        }

        private void CheckTwoDigitWord(ref bool[] typeVal, Match m, int line, int col, string type)
        {
            string pattern = type + @"\d{1,2}";
            m = Regex.Match(m.Value, pattern, RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                Errors.Add(new Error(line + 1, col, type + " word is not valid"));
                return;
            }
            pattern = @"\d{1,2}";
            m = Regex.Match(m.Value, pattern);

            if (!m.Success)
            {
                Errors.Add(new Error(line + 1, col, type + " word is not valid"));
                return;
            }
            short x = -1;
            if (!Int16.TryParse(m.Value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out x))
            {
                Errors.Add(new Error(line + 1, col, type + " word is not valid"));
            }
            else
                typeVal[x] = true;
        }

        private void CheckFourDigitWord(out short typeVal, Match m, int line, int col, string type)
        {
            typeVal = -1;
            if (type == "N" && m.Index != 0)
            {
                Errors.Add(new Error(line + 1, col, "N word has to be at the beginning of a sentence"));
                return;
            }
            string pattern = type + @"\d{1,4}";
            m = Regex.Match(m.Value, pattern, RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                Errors.Add(new Error(line + 1, col, type + " word is not valid"));
                return;
            }
            pattern = @"\d{1,4}";
            m = Regex.Match(m.Value, pattern);
            if (!m.Success)
            {
                Errors.Add(new Error(line + 1, col, type + " word is not valid"));
                return;
            }
            short x = -1;
            if (!Int16.TryParse(m.Value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out x))
            {
                Errors.Add(new Error(line + 1, col, type + " word is not valid"));
            }
            else
                typeVal = x;
        }

        private void CheckDecimalWord(out double typeVal, Match m, int line, int col, string type)
        {
            typeVal = 0;
            string pattern = type + @"[-+]?[0-9]*\.?[0-9]+";
            m = Regex.Match(m.Value, pattern, RegexOptions.IgnoreCase);
            if (!m.Success)
            {
                Errors.Add(new Error(line + 1, col, type + " word is not valid"));
                return;
            }
            pattern = @"[-+]?[0-9]*\.?[0-9]+";
            m = Regex.Match(m.Value, pattern);
            double x = -1;
            if (!Double.TryParse(m.Value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out x))
            {
                Errors.Add(new Error(line + 1, col, type + " word is not valid"));
            }
            else
                typeVal = x;
        }

        internal bool IsErrorFree { get; set; }

        internal class Error
        {
            internal Error(int line, int col, string msg)
            {
                StringType = String.Format(errMsgFormat, line, col, msg);
            }
            public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text
            private string StringType = String.Empty;

            public override string ToString()
            {
                return StringType;
            }
        }

        internal List<Error> Errors = new List<Error>();
    }
}
