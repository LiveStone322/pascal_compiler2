using System;
using System.Collections.Generic;
using System.Text;
using PascalCompiler2.Helpers;

namespace PascalCompiler2
{
    class Semantic
    {
        ErrorController errorController;
        List<Token> tokens;

        public Semantic(List<Token> tokens, ErrorController ec)
        {
            this.tokens = tokens;
            errorController = ec;
        }

        public void Start()
        {
        }
    }
}
