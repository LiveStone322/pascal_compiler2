using System;
using PascalCompiler2.Helpers;

namespace PascalCompiler2
{
    class Program
    {
        const string FILE_NAME = "1.pas";

        static IOController ioController = new IOController(FILE_NAME);

        static void Main(string[] args)
        {
            // controllers
            CodeController codeController = new CodeController(ioController);
            ErrorController errorController = new ErrorController();

            /*
             * lexer
             */
            printPhase("Лексический анализатор");
            var lexer = new Lexer(ioController, codeController, errorController);
            var tokens = lexer.List();

            if (errorController.AnyErrors())
            {
                ioController.WriteLine("Возникли ошибки...");
                ioController.WriteLine(errorController.GetAllErrors());
                goto Finish;
            }
            ioController.WriteLine(lexer.GetAllTokens());

            /*
             * syntaxer
             */
            printPhase("Синтаксический анализатор");
            var syntaxer = new Syntaxer(tokens, errorController);

            /*
             * close files
             */
            Finish:
            ioController.Dispose();
        }

        private static void PrintLine(int length = 16)
        {
            for (int i = 0; i < length; i++)
                ioController.Write("_");
            ioController.WriteLine();
            ioController.WriteLine();
        }

        private static void printPhase(string phaseName)
        {
            PrintLine(phaseName.Length + 4);
            ioController.WriteLine("| " + phaseName + " |");
            PrintLine(phaseName.Length + 4);
        }
    }
}
