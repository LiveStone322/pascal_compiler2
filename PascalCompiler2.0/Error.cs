using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler2
{
    class Error
    {
        int lineIndex;
        int charInLineIndex;
        int newCharInLineIndex;
        Models.ErrorCodes errorCode;

        Models.ErrorTypes type;

        string expected;
        string got;
        string codeLine;

        const int MAX_CODE_LENGTH = 60;
        const int MAX_CODE_LENGTH_HALF = MAX_CODE_LENGTH / 2;

        public Error(int lineIndex, int charInLineIndex, Models.ErrorCodes errorCode,
            Models.ErrorTypes type = Models.ErrorTypes.Lex, string codeLine = "", string expected = "", string got = "")
        {
            this.lineIndex = lineIndex;
            this.charInLineIndex = newCharInLineIndex = charInLineIndex;
            this.errorCode = errorCode;
            this.type = type;

            this.codeLine = codeLine;
            this.expected = expected;
            this.got = got;

            if (codeLine.Length > MAX_CODE_LENGTH)
            {
                var startIndex = (charInLineIndex - MAX_CODE_LENGTH_HALF < 0) 
                    ? 0 
                    : (charInLineIndex + MAX_CODE_LENGTH_HALF >= codeLine.Length) 
                        ? codeLine.Length - MAX_CODE_LENGTH 
                        : (charInLineIndex - MAX_CODE_LENGTH_HALF);

                newCharInLineIndex = charInLineIndex - startIndex;
                this.codeLine = this.codeLine.Substring(startIndex, MAX_CODE_LENGTH);
            }
        }

        public string GetErrorString()
        {
            switch (type)
            {
                case Models.ErrorTypes.Lex:
                    return $"| ОШИБКА E{(int)errorCode} на ({lineIndex}, {charInLineIndex}): {GetErrorDesciprion()}\n" +
                      $"| {codeLine}\n" + 
                      $"| {getCodePointer()}\n";
                case Models.ErrorTypes.Synt:
                    return $"| ОШИБКА E{(int)errorCode} на ({lineIndex}, {charInLineIndex}): Ожидалось {expected}, получили {got}\n";
                case Models.ErrorTypes.Sem:
                    return $"| ОШИБКА E{(int)errorCode} на ({lineIndex}, {charInLineIndex}): Ожидалось {expected}, получили {got}\n";
                default: return "";
            }
        }

        private string GetErrorDesciprion()
        {
            return Models.descriptions.GetValueOrDefault(errorCode, "");
        }

        private string getCodePointer()
        {
            string res = "";
            for (int i = 0; i < codeLine.Length; i++)
            {
                if (i != newCharInLineIndex) res += "_";
                else res += "^";
            }
            return res;
        }
    }
}
