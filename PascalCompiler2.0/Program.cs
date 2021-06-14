using System;
using PascalCompiler2.Helpers;

namespace PascalCompiler2
{
    class Program
    {
        static readonly string[] FILE_NAMES = new string[] { "Src\\1.pas", "Src\\2.pas", "Src\\3.pas", "Src\\4.pas"};

        static IOController ioController;

        static void Main(string[] args)
        {
            foreach (var f in FILE_NAMES)
            {
                try
                {
                    ioController = new IOController(f);
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
                        continue; 
                    }
                    else ioController.WriteLine("Ошибок нет\n");
                    ioController.WriteLine(lexer.GetAllTokens());

                    /*
                     * syntaxer
                     */
                    printPhase("Синтаксический и семантический анализатор");
                    var syntaxer = new SemSyntaxer(tokens, errorController);
                    syntaxer.Start();

                    if (errorController.AnyErrors())
                    {
                        ioController.WriteLine("Возникли ошибки...");
                        ioController.WriteLine(errorController.GetAllErrors());
                        continue;
                    }
                    else ioController.WriteLine("Ошибок нет\n");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка: " + e.Message);
                }
                finally
                {
                    /*
                     * close files
                     */
                    ioController.Dispose();
                }
            }
            
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
