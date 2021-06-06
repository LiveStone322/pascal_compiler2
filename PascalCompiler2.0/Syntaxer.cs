using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler2
{
    class Syntaxer
    {
        LinkedList<Token> tokens;
        LinkedListNode<Token> curTokenNode;
        ErrorController errorController;

        public Syntaxer (List<Token> tokens, ErrorController ec)
        {
            this.tokens = new LinkedList<Token>(tokens);
            errorController = ec;
            curTokenNode = this.tokens.First;
        }

        private Token token
        {
            get
            {
                return curTokenNode.Value;
            }
        }

        private Models.Tags symbol
        {
            get
            {
                return curTokenNode.Value.tag;
            }
        }

        private void NextSym()
        {
            curTokenNode = curTokenNode.Next;
        }

        private void Accept(Models.Tags sym)
        {
            if (symbol == sym)
                NextSym();
            else errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.SYNTAX_ERROR, Models.ErrorTypes.Synt, "", sym.ToString(), symbol.ToString());
        }

        private void VarOrFunc()
        {
            Accept(Models.Tags.IDENT);
            while (symbol == Models.Tags.LEFT_SQ_BRACKET || symbol == Models.Tags.POINT || symbol == Models.Tags.ARROW || symbol ==  Models.Tags.LEFT_BRACKET)
            {
                switch (symbol)
                {
                    case Models.Tags.LEFT_SQ_BRACKET:
                        NextSym();
                        Expression();
                        while (symbol == Models.Tags.COMMA)
                        {
                            NextSym();
                            Expression();
                        }
                        Accept(Models.Tags.RIGHT_SQ_BRACKET);
                        break;
                    case Models.Tags.POINT:
                        NextSym();
                        Accept(Models.Tags.IDENT);
                        break;
                    case Models.Tags.ARROW:
                        NextSym();
                        break;
                    case Models.Tags.LEFT_BRACKET:
                        NextSym();
                        if (symbol != Models.Tags.RIGHT_BRACKET) Expression();
                        while (symbol == Models.Tags.COMMA)
                        {
                            NextSym();
                            Expression();
                        }
                        Accept(Models.Tags.RIGHT_BRACKET);
                        break;
                }
            }
                
        }

        private void Expression()
        {

            if (symbol == Models.Tags.PLUS || symbol == Models.Tags.MINUS)
            {
                NextSym();

            }
        }

        private void Statement()
        {
            if (symbol == Models.Tags.PLUS || symbol == Models.Tags.MINUS)
                Expression();
            else
            {
                switch (symbol)
                {
                    case Models.Tags.IDENT:
                        NextSym();
                        while (symbol == Models.Tags.LEFT_SQ_BRACKET || symbol == Models.Tags.POINT
                                || symbol == Models.Tags.ARROW || symbol == Models.Tags.LEFT_BRACKET)
                        {
                            switch (symbol)
                            {
                                case Models.Tags.LEFT_SQ_BRACKET:
                                    NextSym();
                                    Expression();
                                    while (symbol == Models.Tags.COMMA)
                                    {
                                        NextSym();
                                        Expression();
                                    }
                                    Accept(Models.Tags.RIGHT_SQ_BRACKET);
                                    break;
                                case Models.Tags.POINT:
                                    NextSym();
                                    Accept(Models.Tags.IDENT);
                                    break;
                                case Models.Tags.ARROW:
                                    NextSym();
                                    break;
                                case Models.Tags.LEFT_BRACKET:
                                    NextSym();
                                    if (symbol != Models.Tags.RIGHT_BRACKET) Expression();
                                    while (symbol == Models.Tags.COMMA)
                                    {
                                        NextSym();
                                        Expression();
                                    }
                                    Accept(Models.Tags.RIGHT_BRACKET);
                                    break;
                            }
                        }
                        if (symbol == Models.Tags.ASSIGN)
                        {
                            NextSym();
                            if (symbol != Models.Tags.ENDSY && symbol != Models.Tags.SEMICOLON)
                                Expression();
                        }
                        break;
                    case Models.Tags.IFSY:
                        IfStatement();
                        break;
                    case Models.Tags.WHILESY:
                        WhileStatement();
                        break;
                    case Models.Tags.FORSY:
                        ForStatement();
                        break;
                }
            }
        }

        private void BigBlock()
        {
            Accept(Models.Tags.BEGINSY);
            Statement();
            while (symbol == Models.Tags.SEMICOLON)
            {
                NextSym();
                if (symbol != Models.Tags.ENDSY) Statement();
            }
            Accept(Models.Tags.ENDSY);
        }

        private void BigOrSmallBlock()
        {
            if (symbol == Models.Tags.BEGINSY)
            {
                NextSym();
                if (symbol != Models.Tags.ENDSY)
                {
                    Statement();
                    while (symbol == Models.Tags.SEMICOLON)
                    {
                        NextSym();
                        if (symbol != Models.Tags.ENDSY) Statement();
                    }
                }
                Accept(Models.Tags.ENDSY);
                if (symbol == Models.Tags.SEMICOLON) Accept(Models.Tags.SEMICOLON);
            }
            else
            {
                Statement();
            }
        }

        private void TypePart()
        {
            if (symbol == Models.Tags.TYPESY)
            {
                Accept(Models.Tags.TYPESY);
                do
                {
                    TypeDeclaration();
                    Accept(Models.Tags.SEMICOLON);
                }
                while (symbol == Models.Tags.IDENT);
            }
        }

        private void TypeDeclaration()
        {
            Accept(Models.Tags.IDENT);
            while (symbol == Models.Tags.COMMA)
            {
                NextSym();
                Accept(Models.Tags.IDENT);
            }
            Accept(Models.Tags.EQUAL);
            Type();
        }

        private void VarPart()
        {
            if (symbol == Models.Tags.VARSY)
            {
                Accept(Models.Tags.VARSY);
                do
                {
                    VarDeclaration();
                    Accept(Models.Tags.SEMICOLON);
                }
                while (symbol == Models.Tags.IDENT);
            }
        }

        private void VarDeclaration()
        {
            Accept(Models.Tags.IDENT);
            while (symbol == Models.Tags.COMMA)
            {
                NextSym();
                Accept(Models.Tags.IDENT);
            }
            Accept(Models.Tags.COLON);
            Type();
        }

        private void Type()
        {
            var types = new string[] { "integer", "real", "boolean", "char", "string" };
            if (!Array.Exists(types, t => t == token.val)) 
                errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.SYNTAX_ERROR, 
                    Models.ErrorTypes.Synt, "", "тип", token.val);
        }

        private void SimpleType()
        {
           
        }

        private void IfStatement()
        {
            Accept(Models.Tags.IFSY); 
            Expression();
            Accept(Models.Tags.THENSY);
            BigOrSmallBlock();
            if (symbol == Models.Tags.ELSESY)
            {
                NextSym();
                BigOrSmallBlock();
            }
        }

        private void WhileStatement()
        {
            Accept(Models.Tags.WHILESY); 
            Expression();
            Accept(Models.Tags.DOSY);
            BigOrSmallBlock();
        }

        private void ForStatement()
        {
            Accept(Models.Tags.FORSY); 
            Accept(Models.Tags.IDENT);
            Accept(Models.Tags.ASSIGN); 
            Expression();
            if (symbol == Models.Tags.TOSY || symbol == Models.Tags.DOWNTOSY)
                NextSym();
            Expression(); Accept(Models.Tags.DOSY);
            BigOrSmallBlock();
        }

        private void Programme()
        {
            Accept(Models.Tags.PROGRAMSY);
            Accept(Models.Tags.IDENT);
            Accept(Models.Tags.SEMICOLON);
            TypePart();
            VarPart();
            BigBlock(); 
            Accept(Models.Tags.POINT);
        }

        public void Start()
        {
            Programme();
        }
    }
}
