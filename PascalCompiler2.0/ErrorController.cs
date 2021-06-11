using System.Collections.Generic;

namespace PascalCompiler2
{
    class ErrorController
    {
        List<Error> errors = new List<Error>();

        public void AddError(int lineIndex, int charInLineInde, Models.ErrorCodes errorCode, string codeLine = "", string info = "")
        {
            errors.Add(new Error(lineIndex, charInLineInde, errorCode, codeLine, info));
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
