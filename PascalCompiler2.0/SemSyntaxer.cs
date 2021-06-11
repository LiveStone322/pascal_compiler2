using System;
using System.Collections.Generic;
using System.Text;
using PascalCompiler2.Helpers;

namespace PascalCompiler2
{
    class SemSyntaxer
    {
        LinkedList<Token> tokens;
        LinkedListNode<Token> curTokenNode;
        ErrorController errorController; 
        IdentScopeHelper ish;

        public SemSyntaxer (List<Token> tokens, ErrorController ec)
        {
            this.tokens = new LinkedList<Token>(tokens);
            errorController = ec;
            curTokenNode = this.tokens.First;
            ish = new IdentScopeHelper();
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

        private string Accept(Models.Tags sym, Models.Places place)
        {
            if (symbol == sym)
            {
                var val = token.val;
                NextSym();
                return val;
            }
            errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.SYNTAX_ERROR, "",
                $"Ожидалось {sym}, полчили { symbol }");
            Skip(Models.followers.GetValueOrDefault(place, new Models.Tags[] { }));
            return "";
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

        private void Expression(TypeTableEntity type_ = null)
        {
            if (isPLusMinusOperation(symbol))
            {
                NextSym();
                ArithmeticExpression();
            }
            if (symbol == Models.Tags.NOTSY)
            {
                if (!type_.IsCompatableTo(GetTypeFromBasicType(Models.Tags.BOOLEAN_CONST)))
                {
                    errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.WRONG_TYPE);
                    Skip(Models.followers[Models.Places.EXPRESSION]);
                    return;
                }
                NextSym();
                BooleanExpression();
            }
            else
            {
                switch (symbol)
                {
                    case Models.Tags.IDENT:
                        var ident = ish.cur.FindIdent(token.val);
                        VarOrFunc();
                        if (isBiArithmeticOperation(symbol))
                        {
                            NextSym();
                            ArithmeticExpression(ident.Type);
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
                            if (type_.IsCompatableTo(GetTypeFromBasicType(symbol)))
                            {
                                errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.WRONG_TYPE);
                                Skip(Models.followers[Models.Places.EXPRESSION]);
                                return;
                            }
                            ArithmeticExpression(GetTypeFromBasicType(symbol));
                        } 
                        else if (isBooleanOperation(symbol))
                        {
                            if (!type_.IsCompatableTo(GetTypeFromBasicType(Models.Tags.BOOLEAN_CONST)))
                            {
                                errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.WRONG_TYPE);
                                Skip(Models.followers[Models.Places.EXPRESSION]);
                                return;
                            }
                            NextSym();
                            BooleanExpression(true);
                        }
                        break;
                }
            }
        }

        private void BooleanExpression(bool acceptNonBoolean = false)
        {
            var boolType = ish.cur.FindType("boolean");
            switch (symbol)
            {
                case Models.Tags.IDENT:
                    var ident = ish.cur.FindIdent(token.val);
                    VarOrFunc();
                    if (ident == null)
                    {
                        errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.UNKNOWN_IDENT);
                        Skip(Models.followers[Models.Places.STATEMENT]);
                        return;
                    }
                    var typeIdent = ident.Type;
                    if (!acceptNonBoolean && !boolType.IsCompatableTo(typeIdent))
                    {
                        errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.WRONG_TYPE, "", "ожидался тип " + boolType.Name);
                        Skip(Models.followers[Models.Places.STATEMENT]);
                        return;
                    }
                    VarOrFunc();

                    if (isCompareOperation(symbol))
                    {
                        NextSym();
                        BooleanExpression(true);
                    } else if (isBooleanOperation(symbol))
                    {
                        NextSym();
                        BooleanExpression();
                    }
                    break;
                case Models.Tags.NOTSY:
                    NextSym();
                    BooleanExpression();
                    break;
                case Models.Tags.INT_CONST:
                case Models.Tags.FLOAT_CONST:
                case Models.Tags.CHAR_CONST:
                case Models.Tags.STRING_CONST:
                case Models.Tags.BOOLEAN_CONST:
                    if (!acceptNonBoolean && symbol != Models.Tags.BOOLEAN_CONST)
                    {
                        errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.WRONG_TYPE, "", "ожидался тип " + boolType.Name);
                        Skip(Models.followers[Models.Places.STATEMENT]);
                        return;
                    }
                    NextSym();
                    if (isCompareOperation(symbol))
                    {
                        NextSym();
                        BooleanExpression(true);
                    }
                    else if (isBooleanOperation(symbol))
                    {
                        NextSym();
                        BooleanExpression();
                    }
                    break;
            }
        }

        private void ArithmeticExpression(TypeTableEntity type_ = null)
        {
            TypeTableEntity type = null;
            switch (symbol)
            {
                case Models.Tags.IDENT:
                    var ident = ish.cur.FindIdent(token.val);
                    VarOrFunc();
                    if (ident == null)
                    {
                        errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.UNKNOWN_IDENT);
                        Skip(Models.followers[Models.Places.STATEMENT]);
                        return;
                    }
                    type = ident.Type;
                    if (!type_.IsCompatableTo(type))
                    {
                        errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.WRONG_TYPE, "", "ожидался тип " + type_.Name);
                        Skip(Models.followers[Models.Places.STATEMENT]);
                        return;
                    }
                    VarOrFunc();
                    if (isBiArithmeticOperation(symbol))
                    {
                        NextSym();
                        ArithmeticExpression(type);
                    }
                    break;
                case Models.Tags.INT_CONST:
                case Models.Tags.FLOAT_CONST:
                case Models.Tags.CHAR_CONST:
                case Models.Tags.STRING_CONST:
                    if (!type_.IsCompatableTo(GetTypeFromBasicType(symbol)))
                    {
                        errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.WRONG_TYPE, "", "ожидался тип " + type_.Name);
                        Skip(Models.followers[Models.Places.STATEMENT]);
                        return;
                    }
                    NextSym();
                    if (isBiArithmeticOperation(symbol))
                    {
                        NextSym();
                        ArithmeticExpression(GetTypeFromBasicType(symbol));
                    }
                    break;
                default:
                    Console.WriteLine(symbol + " что-то не так");
                    break;
            }
        }

        private TypeTableEntity GetTypeFromBasicType(Models.Tags t)
        {
            switch (t)
            {
                case Models.Tags.INT_CONST:
                    return ish.cur.FindType("integer");
                case Models.Tags.FLOAT_CONST:
                    return ish.cur.FindType("real");
                case Models.Tags.CHAR_CONST:
                    return ish.cur.FindType("char");
                case Models.Tags.STRING_CONST:
                    return ish.cur.FindType("string");
                case Models.Tags.BOOLEAN_CONST:
                    return ish.cur.FindType("boolean");
            }
            return null;
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
            return Array.Exists(Models.booleanOperatorsExceptNot, t => t == s)
                || Array.Exists(Models.compareOperators, t => t == s);
        }

        private bool isCompareOperation(Models.Tags s)
        {
            return Array.Exists(new Models.Tags[] { Models.Tags.GREATER, Models.Tags.GREATER_EQ, Models.Tags.LESS, Models.Tags.LESS_EQ, Models.Tags.EQUAL }, t => t == s);
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
                        var ident = ish.cur.FindIdent(Accept(Models.Tags.IDENT, Models.Places.STATEMENT));
                        if (ident == null)
                        {
                            errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.UNKNOWN_IDENT);
                            Skip(Models.followers[Models.Places.STATEMENT]);
                            return;
                        }

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
                                Expression(ident.Type);
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
            List<string> names = new List<string>();
            names.Add(Accept(Models.Tags.IDENT, Models.Places.TYPE_DECLARATION));

            while (symbol == Models.Tags.COMMA)
            {
                NextSym();
                names.Add(Accept(Models.Tags.IDENT, Models.Places.TYPE_DECLARATION));
            }
            Accept(Models.Tags.EQUAL, Models.Places.TYPE_DECLARATION);
            var type = Type();
            foreach (var n in names)
            {
                ish.cur.AddType(n, type);
            }
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
            List<string> names = new List<string>();
            names.Add(Accept(Models.Tags.IDENT, Models.Places.VAR_DECLARATION));
            while (symbol == Models.Tags.COMMA)
            {
                NextSym();
                names.Add(Accept(Models.Tags.IDENT, Models.Places.VAR_DECLARATION));
            }
            Accept(Models.Tags.COLON, Models.Places.VAR_DECLARATION);
            var type = Type();
            if (type == null)
                Skip(Models.followers[Models.Places.TYPE]);
            else
            {
                foreach (var n in names)
                {
                    ish.cur.AddIdent(n, IdentUsageType.VAR, type);
                }
            }
        }

        private TypeTableEntity Type()
        {
            switch (symbol)
            {
                case Models.Tags.IDENT:
                    var identVal = token.val;
                    NextSym();
                    if (symbol == Models.Tags.SEMICOLON)
                        return SimpleType(identVal);
                    else if (symbol == Models.Tags.TWO_POINTS)
                        return SubrangeType(identVal);
                    break;
                case Models.Tags.LEFT_BRACKET:
                    return EnumType();
                case Models.Tags.CHAR_CONST:
                case Models.Tags.INT_CONST:
                case Models.Tags.FLOAT_CONST:
                    return SubrangeType();
                case Models.Tags.ARRAYSY:
                    return ArrayType();
            }
            return null;
        }

        private TypeTableEntity SimpleType(string initValue = null)
        {
            TypeTableEntity lookupType = null;

            if (initValue != null) lookupType = ish.cur.FindType(initValue);
            else lookupType = ish.cur.FindType(token.val);

            if (lookupType == null)
            {
                errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.UNKNOWN_TYPE);
                return null;
            }
            return lookupType;
        }

        private TypeTableEntity EnumType(string initValue = null)
        {
            List<IdentTableEntity> names = new List<IdentTableEntity>();
            if (initValue != null) names.Add(new IdentTableEntity(IdentUsageType.TYPE, initValue));
            else names.Add(new IdentTableEntity(null, IdentUsageType.TYPE, Accept(Models.Tags.IDENT, Models.Places.VAR_DECLARATION)));
            while (symbol == Models.Tags.COMMA)
            {
                NextSym();
                names.Add(new IdentTableEntity(null, IdentUsageType.TYPE, Accept(Models.Tags.IDENT, Models.Places.VAR_DECLARATION)));
            }
            Accept(Models.Tags.RIGHT_BRACKET, Models.Places.VAR_DECLARATION);
            return new TypeTableEntity(TypeTableTypeEnums.TYPECLASS_ENUM, "", new EnumTypeParams(names.ToArray()));
        }

        private bool IsChar(string s)
        {
            return s.Length == 1;
        }

        private bool IsInt(string s)
        {
            int value;
            if (int.TryParse(s, out value))
                return true;
            return false;
        }

        private bool IsDouble(string s)
        {
            double value;
            if (double.TryParse(s, out value))
                return true;
            return false;
        }

        private TypeTableEntity SubrangeType(string initValue = null)
        {
            string from = "", to = "";
            switch (symbol)
            {
                case Models.Tags.CHAR_CONST:
                    from = Accept(Models.Tags.CHAR_CONST, Models.Places.TYPE_DECLARATION);
                    Accept(Models.Tags.TWO_POINTS, Models.Places.VAR_DECLARATION);
                    to = Accept(Models.Tags.CHAR_CONST, Models.Places.TYPE_DECLARATION);
                    if (IsChar(from) && IsChar(to))
                        return new TypeTableEntity(TypeTableTypeEnums.TYPECLASS_LIMITED, "", new LimitedTypeParams(from[0], to[0]));
                    break;
                case Models.Tags.INT_CONST:
                    from = Accept(Models.Tags.INT_CONST, Models.Places.TYPE_DECLARATION);
                    Accept(Models.Tags.TWO_POINTS, Models.Places.VAR_DECLARATION);
                    to = Accept(Models.Tags.INT_CONST, Models.Places.TYPE_DECLARATION);
                    if (IsInt(from) && IsInt(to))
                        return new TypeTableEntity(TypeTableTypeEnums.TYPECLASS_LIMITED, "", new LimitedTypeParams(int.Parse(from), int.Parse(to)));
                    break;
            }
            return null;
        }

        private TypeTableEntity ArrayType(string initValue = null)
        {
            List<TypeTableEntity> names = new List<TypeTableEntity>();
            Accept(Models.Tags.ARRAYSY, Models.Places.TYPE_DECLARATION);
            Accept(Models.Tags.LEFT_SQ_BRACKET, Models.Places.TYPE_DECLARATION);

            do
            {
                if (symbol == Models.Tags.COMMA) NextSym();
                names.Add(Type());
            } while (symbol == Models.Tags.COMMA);

            Accept(Models.Tags.RIGHT_SQ_BRACKET, Models.Places.TYPE_DECLARATION);
            Accept(Models.Tags.OFSY, Models.Places.TYPE_DECLARATION);
            var type = Type();

            return new TypeTableEntity(TypeTableTypeEnums.TYPECLASS_REGULAR, "", new RegularTypeParams(type, names.ToArray()));
        }

        private void IfStatement()
        {
            Accept(Models.Tags.IFSY, Models.Places.IF); 
            Expression();
            Accept(Models.Tags.THENSY, Models.Places.IF);
            BigOrSmallBlock();
            if (symbol == Models.Tags.SEMICOLON) NextSym();
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
                errorController.AddError(token.lineIndex, token.charIndex, Models.ErrorCodes.SYNTAX_ERROR, "",
                    $"Ожидалось {Models.Tags.TOSY} или {Models.Tags.DOWNTOSY}, полчили { symbol }");
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
            var name = Accept(Models.Tags.IDENT, Models.Places.PROGRAM);
            ish.cur.AddIdent(name, IdentUsageType.ROOT);
            ish.OpenScope();
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
