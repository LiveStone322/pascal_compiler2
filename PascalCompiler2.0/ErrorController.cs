using System.Collections.Generic;

namespace PascalCompiler2
{
    class ErrorController
    {
        List<Error> errors = new List<Error>();

        public void AddError(int lineIndex, int charInLineInde, Models.ErrorCodes errorCode, 
            Models.ErrorTypes type, string codeLine = "", string expected = "", string got = "")
        {
            switch (type)
            {
                case Models.ErrorTypes.Lex:
                    errors.Add(new Error(lineIndex, charInLineInde, errorCode, type, codeLine));
                    break;
                case Models.ErrorTypes.Synt:
                    errors.Add(new Error(lineIndex, charInLineInde, errorCode, type, "", expected, got));
                    break;
                case Models.ErrorTypes.Sem:
                    errors.Add(new Error(lineIndex, charInLineInde, errorCode, type, "", expected, got));
                    break;
            }
        }

        public string GetAllErrors()
        {
            var result = "";
            foreach(var e in errors)
            {
                result += e.GetErrorString() + "\n";
            }
            return result;
        }

        public void Clear()
        {
            errors.Clear();
        }

        public bool AnyErrors()
        {
            return errors.Count > 0;
        }
    }
}
