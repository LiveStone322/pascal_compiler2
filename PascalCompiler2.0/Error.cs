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

        string info;
        string codeLine;

        const int MAX_CODE_LENGTH = 60;
        const int MAX_CODE_LENGTH_HALF = MAX_CODE_LENGTH / 2;

        public Error(int lineIndex, int charInLineIndex, Models.ErrorCodes errorCode, string codeLine = "", string info = "")
        {
            this.lineIndex = lineIndex + 1;
            this.charInLineIndex = newCharInLineIndex = charInLineIndex + 1;
            this.errorCode = errorCode;

            this.codeLine = codeLine;
            this.info = info;

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
            return $"| ОШИБКА E{(int)errorCode} {( (lineIndex == -1 && charInLineIndex == -1) ? "" : $"на ({lineIndex}, {charInLineIndex})")}: {GetErrorDesciprion()}\n" +
                      (codeLine != "" ? $"| {codeLine}\n" : "") +
                      (codeLine != "" ? $"| {getCodePointer()}\n" : "") +
                      (info != "" ? $"| {info}\n" : "");
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
