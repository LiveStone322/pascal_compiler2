using System;
using System.Collections.Generic;

namespace PascalCompiler2
{
    class Lexer
    {
        IOController io;
        CodeController cc;
        ErrorController ec;

        List<Token> tokens = new List<Token>();


        string lex = "";
        int charIndex = 0;
        int lineIndex = 0;

        public Lexer(IOController io, CodeController cc, ErrorController ec)
        {
            this.io = io;
            this.cc = cc;
            this.ec = ec;
        }

        private bool isDelimeter(char ch)
        {
           return Array.Exists(Models.delimeters, t => t == ch);
        }

        private bool isWhitespace(char ch)
        {
            if (ch == '\t' || ch == ' ')
                return true;
            else
                return false;
        }

        private bool isLetter(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_';
        }

        private bool isNum(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private char curChar
        {
            get
            {
                return cc.curCharHelper.cur;
            }
        }

        private char nextChar
        {
            get
            {
                return cc.curCharHelper.next;
            }
        }

        private Token NewToken(Models.Tags tag, bool needLexeme = true)
        {
            return new Token(tag, needLexeme ? lex : "", lineIndex, charIndex);
        }

        private Models.Tags OprimizedKeywordSearch(string searchValue)
        {
            if (searchValue == "true" || searchValue == "false") return Models.Tags.BOOLEAN_CONST;
            foreach (var k in Models.keywords)
            {
                if (k.Key == searchValue) return k.Value;
            }
            return Models.Tags.IDENT;
        }

        public string GetAllTokens()
        {
            var result = "";
            foreach (var e in tokens)
            {
               result += $"{e.lineIndex}:{e.charIndex}{ (e.lineIndex < 10 && e.charIndex < 10 ? "\t" : "") }\t{e.tag} {(e.tag == Models.Tags.IDENT ? e.val : "")}\n";
            }
            return result;
        }

        public List<Token> List()
        {
            do
            {
                Token token = null;

                lex = "";
                charIndex = cc.posHelper.x;
                lineIndex = cc.posHelper.y;

                if (isWhitespace(curChar))
                {
                    continue;
                }
                if (curChar == '\"')
                {
                    cc.NextChar();
                    lex += GetLexeme(keepWhitespaces: true);
                    if (nextChar != '\"')
                    {
                        ec.AddError(lineIndex, charIndex, Models.ErrorCodes.NO_ENDING_DOUBLE_QUOTE, cc.curLine);
                        continue;
                    }
                    else
                    {
                        token = NewToken(Models.Tags.STRING_CONST);
                        cc.NextChar();
                    }
                }
                else if (curChar == '\'')
                {
                    lex += GetLexeme();
                    if (nextChar != '\'')
                    {

                        ec.AddError(lineIndex, charIndex, Models.ErrorCodes.NO_ENDING_SINGLE_QUOTE, cc.curLine);
                        continue;
                    }
                    else
                    {
                        token = NewToken(Models.Tags.CHAR_CONST);
                        cc.NextChar();
                    }
                }
                else if (isNum(curChar))
                {
                    lex += GetLexeme();
                    // if float
                    if (nextChar == '.')
                    {
                        cc.NextChar();
                        var floatPart = GetLexeme();
                        if (floatPart == ".")
                        {
                            ec.AddError(lineIndex, charIndex, Models.ErrorCodes.CONST_ERROR, cc.curLine);
                            continue;
                        }
                        lex += '.' + floatPart;
                        try
                        {
                            double.Parse(lex);
                            token = NewToken(Models.Tags.FLOAT_CONST);
                        }
                        catch(Exception)
                        {
                            ec.AddError(lineIndex, charIndex, Models.ErrorCodes.CONST_ERROR, cc.curLine);
                            continue;
                        }
                    }
                    else
                    {
                        try
                        {
                            int.Parse(lex);
                            token = NewToken(Models.Tags.INT_CONST);
                        }
                        catch (Exception)
                        {
                            ec.AddError(lineIndex, charIndex, Models.ErrorCodes.CONST_ERROR, cc.curLine);
                            continue;
                        }
                    }
                }
                else if (isLetter(curChar))
                {
                    lex += GetLexeme();
                    var tag = OprimizedKeywordSearch(lex);
                    token = NewToken(tag, tag == Models.Tags.IDENT);
                }
                else
                {
                    switch (curChar)
                    {
                        case ';':
                            token = NewToken(Models.Tags.SEMICOLON, false);
                            break;
                        case '(':
                            token = NewToken(Models.Tags.LEFT_BRACKET, false);
                            break;
                        case ')':
                            token = NewToken(Models.Tags.RIGHT_BRACKET, false);
                            break;
                        case '[':
                            token = NewToken(Models.Tags.LEFT_SQ_BRACKET, false);
                            break;
                        case ']':
                            token = NewToken(Models.Tags.RIGHT_SQ_BRACKET, false);
                            break;
                        case '{':
                            token = NewToken(Models.Tags.LEFT_F_BRACKET, false);
                            break;
                        case '}':
                            token = NewToken(Models.Tags.RIGHT_F_BRACKET, false);
                            break;
                        case ',':
                            token = NewToken(Models.Tags.COMMA, false);
                            break;
                        case ':':
                            if (nextChar == '=')
                            {
                                token = NewToken(Models.Tags.ASSIGN, false);
                                cc.NextChar();
                            }
                            else token = NewToken(Models.Tags.COLON, false);
                            break;
                        case '<':
                            if (nextChar == '=')
                            {
                                token = NewToken(Models.Tags.LESS_EQ, false);
                                cc.NextChar();
                            }
                            else
                            {
                                if (nextChar == '>')
                                {
                                    token = NewToken(Models.Tags.NOT_EQ, false);
                                    cc.NextChar();
                                }
                                else token = NewToken(Models.Tags.LESS, false);
                            }
                            break;
                        case '>':
                            if (nextChar == '=')
                            {
                                token = NewToken(Models.Tags.GREATER_EQ, false);
                                cc.NextChar();
                            }
                            else token = NewToken(Models.Tags.GREATER, false);
                            break;
                        case '.':
                            if (nextChar == '.')
                            {
                                token = NewToken(Models.Tags.TWO_POINTS, false);
                                cc.NextChar();
                            }
                            else token = NewToken(Models.Tags.POINT, false);
                            break;
                        case '=':
                            token = NewToken(Models.Tags.EQUAL, false);
                            break;
                        case '+':
                            token = NewToken(Models.Tags.PLUS, false);
                            break;
                        case '-':
                            token = NewToken(Models.Tags.MINUS, false);
                            break;
                        case '*':
                            token = NewToken(Models.Tags.STAR, false);
                            break;
                        case '/':
                            token = NewToken(Models.Tags.SLASH, false);
                            break;
                    }
                }
                if (token != null) tokens.Add(token);
            } while (cc.NextChar());
            return tokens;
        }

        private string GetNextLexeme()
        { 
            cc.NextChar();
            return GetLexeme();
        }

        private string GetLexeme(bool keepWhitespaces = false)
        {
            string result = curChar.ToString();
            while(!isDelimeter(nextChar) && (keepWhitespaces || !isWhitespace(nextChar)))
            {
                cc.NextChar();
                result += curChar;
            }

            return result;
        }
    }
}
