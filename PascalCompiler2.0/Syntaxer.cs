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

        private void Accept(Models.Tags sym, Models.Places place)
        {
            if (symbol == sym)
                NextSym();
            else
            {
                errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.SYNTAX_ERROR,
                    Models.ErrorTypes.Synt, "", sym.ToString(), symbol.ToString());
                Skip(Models.followers.GetValueOrDefault(place, new Models.Tags[] { }));
            }
        }

        private void VarOrFunc()
        {
            Accept(Models.Tags.IDENT, Models.Places.EXPRESSION);
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
                        Accept(Models.Tags.RIGHT_SQ_BRACKET, Models.Places.EXPRESSION);
                        break;
                    case Models.Tags.POINT:
                        NextSym();
                        Accept(Models.Tags.IDENT, Models.Places.EXPRESSION);
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
                        Accept(Models.Tags.RIGHT_BRACKET, Models.Places.EXPRESSION);
                        break;
                }
            }
                
        }

        private void Expression()
        {

            if (isPLusMinusOperation(symbol))
            {
                NextSym();
                ArithmeticExpression();
            }
            if (symbol == Models.Tags.NOTSY)
            {
                NextSym();
                BooleanExpression();
            }
            else
            {
                switch (symbol)
                {
                    case Models.Tags.IDENT:
                        VarOrFunc();
                        if (isBiArithmeticOperation(symbol))
                        {
                            NextSym();
                            ArithmeticExpression();
                        }
                        else if (isBooleanOperation(symbol))
                        {
                            NextSym();
                            BooleanExpression();
                        }
                        break;
                    case Models.Tags.BOOLEAN_CONST:
                        NextSym();
                        if (isBooleanOperation(symbol))
                        {
                            NextSym();
                            BooleanExpression();
                        }
                        break;
                    case Models.Tags.INT_CONST:
                    case Models.Tags.FLOAT_CONST:
                    case Models.Tags.CHAR_CONST:
                    case Models.Tags.STRING_CONST:
                        NextSym();
                        if (isBiArithmeticOperation(symbol))
                        {
                            NextSym();
                            ArithmeticExpression();
                        }
                        break;
                }
            }
        }

        private void BooleanExpression()
        {
            switch (symbol)
            {
                case Models.Tags.IDENT:
                    VarOrFunc();
                    if (isBooleanOperation(symbol))
                    {
                        NextSym();
                        BooleanExpression();
                    }
                    break;
                case Models.Tags.NOTSY:
                    NextSym();
                    BooleanExpression();
                    break;
                case Models.Tags.BOOLEAN_CONST:
                    NextSym();
                    if (isBooleanOperation(symbol))
                    {
                        NextSym();
                        BooleanExpression();
                    }
                    break;
            }
        }

        private void ArithmeticExpression()
        {
            switch (symbol)
            {
                case Models.Tags.IDENT:
                    VarOrFunc();
                    if (isBiArithmeticOperation(symbol))
                    {
                        NextSym();
                        ArithmeticExpression();
                    }
                    break;
                case Models.Tags.INT_CONST:
                case Models.Tags.FLOAT_CONST:
                case Models.Tags.CHAR_CONST:
                case Models.Tags.STRING_CONST:
                    NextSym();
                    if (isBiArithmeticOperation(symbol))
                    {
                        NextSym();
                        ArithmeticExpression();
                    }
                    break;
                default:
                    Console.WriteLine(symbol + " что-то не так");
                    break;
            }
        }

        private bool isBiArithmeticOperation(Models.Tags s)
        {
            return Array.Exists(Models.plusMinus, t => t == s)
                || Array.Exists(Models.starSlash, t => t == s)
                || Array.Exists(Models.divMod, t => t == s);
        }

        private bool isPLusMinusOperation (Models.Tags s)
        {
            return Array.Exists(Models.plusMinus, t => t == s);
        }


        private bool isBooleanOperation(Models.Tags s)
        {
            return Array.Exists(Models.booleanOperatorsExceptNot, t => t == s);
        }

        private void Statement()
        {
            if (isPLusMinusOperation(symbol))
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
                                    Accept(Models.Tags.RIGHT_SQ_BRACKET, Models.Places.STATEMENT);
                                    break;
                                case Models.Tags.POINT:
                                    NextSym();
                                    Accept(Models.Tags.IDENT, Models.Places.STATEMENT);
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
                                    Accept(Models.Tags.RIGHT_BRACKET, Models.Places.STATEMENT);
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
            Accept(Models.Tags.BEGINSY, Models.Places.BEGIN_END);
            Statement();
            while (symbol == Models.Tags.SEMICOLON)
            {
                NextSym();
                if (symbol != Models.Tags.ENDSY) Statement();
            }
            Accept(Models.Tags.ENDSY, Models.Places.BEGIN_END);
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
                Accept(Models.Tags.ENDSY, Models.Places.BEGIN_END);
                if (symbol == Models.Tags.SEMICOLON) Accept(Models.Tags.SEMICOLON, Models.Places.BEGIN_END);
            }
            else
            {
                Statement();
                Accept(Models.Tags.SEMICOLON, Models.Places.STATEMENT);
            }
        }

        private void TypePart()
        {
            if (symbol == Models.Tags.TYPESY)
            {
                Accept(Models.Tags.TYPESY, Models.Places.TYPE_BLOCK);
                do
                {
                    TypeDeclaration();
                    Accept(Models.Tags.SEMICOLON, Models.Places.TYPE_BLOCK);
                }
                while (symbol == Models.Tags.IDENT);
            }
        }

        private void TypeDeclaration()
        {
            Accept(Models.Tags.IDENT, Models.Places.TYPE_DECLARATION);
            while (symbol == Models.Tags.COMMA)
            {
                NextSym();
                Accept(Models.Tags.IDENT, Models.Places.TYPE_DECLARATION);
            }
            Accept(Models.Tags.EQUAL, Models.Places.TYPE_DECLARATION);
            Type();
        }

        private void VarPart()
        {
            if (symbol == Models.Tags.VARSY)
            {
                Accept(Models.Tags.VARSY, Models.Places.VAR_BLOCK);
                do
                {
                    VarDeclaration();
                    Accept(Models.Tags.SEMICOLON, Models.Places.VAR_BLOCK);
                }
                while (symbol == Models.Tags.IDENT);
            }
        }

        private void VarDeclaration()
        {
            Accept(Models.Tags.IDENT, Models.Places.VAR_DECLARATION);
            while (symbol == Models.Tags.COMMA)
            {
                NextSym();
                Accept(Models.Tags.IDENT, Models.Places.VAR_DECLARATION);
            }
            Accept(Models.Tags.COLON, Models.Places.VAR_DECLARATION);
            Type();
        }

        private void Type()
        {
            var types = new string[] { "integer", "real", "boolean", "char", "string" };
            if (!Array.Exists(types, t => t == token.val))
            {
                errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.SYNTAX_ERROR,
                    Models.ErrorTypes.Synt, "", "тип", token.val);
            }
            else NextSym();
        }

        private void IfStatement()
        {
            Accept(Models.Tags.IFSY, Models.Places.IF); 
            Expression();
            Accept(Models.Tags.THENSY, Models.Places.IF);
            BigOrSmallBlock();
            if (symbol == Models.Tags.ELSESY)
            {
                NextSym();
                BigOrSmallBlock();
            }
        }

        private void WhileStatement()
        {
            Accept(Models.Tags.WHILESY, Models.Places.WHILE); 
            Expression();
            Accept(Models.Tags.DOSY, Models.Places.WHILE);
            BigOrSmallBlock();
        }

        private void ForStatement()
        {
            var skip = false;
            Accept(Models.Tags.FORSY, Models.Places.FOR); 
            Accept(Models.Tags.IDENT, Models.Places.FOR);
            Accept(Models.Tags.ASSIGN, Models.Places.FOR); 
            Expression();
            if (symbol == Models.Tags.TOSY || symbol == Models.Tags.DOWNTOSY)
                NextSym();
            else
            {
                errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.SYNTAX_ERROR,
                    Models.ErrorTypes.Synt, "", Models.Tags.TOSY.ToString() + " or " + Models.Tags.DOWNTOSY.ToString(), 
                    symbol.ToString());
                Skip(Models.followers.GetValueOrDefault(Models.Places.FOR, new Models.Tags[] { }));
                skip = true;
            }
            if (!skip)
            {
                Expression();
                Accept(Models.Tags.DOSY, Models.Places.FOR);
            }
            else skip = false;
            BigOrSmallBlock();
        }

        private void Programme()
        {
            Accept(Models.Tags.PROGRAMSY, Models.Places.PROGRAM);
            Accept(Models.Tags.IDENT, Models.Places.PROGRAM);
            Accept(Models.Tags.SEMICOLON, Models.Places.PROGRAM);
            TypePart();
            VarPart();
            BigBlock(); 
            Accept(Models.Tags.POINT, Models.Places.PROGRAM);
        }

        private void Skip (Models.Tags[] starters)
        {
            Stack<Models.Tags> closingTags = new Stack<Models.Tags>();
            while (symbol != Models.Tags.ENDSY)
            {
                if (Array.Exists(starters, t => t == symbol))
                    break;
                if (Models.pairs.ContainsKey(symbol))
                {
                    closingTags.Push(symbol);
                    while (closingTags.Count > 0)
                    {
                        NextSym();
                        if (symbol == Models.pairs.GetValueOrDefault(closingTags.Peek(), Models.Tags.POINT))
                        {
                            closingTags.Pop();
                        }
                        else
                        {
                            if (Models.pairs.ContainsKey(symbol))
                                closingTags.Push(symbol);
                        }
                    }
                }
                NextSym();
            }
        }

        public void Start()
        {
            Programme();
        }
    }
}
