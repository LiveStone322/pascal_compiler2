using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler2
{
    static class Models
    {
        public enum Tags
        {
            STAR,               // *
            SLASH,              // /
            EQUAL,              // =
            COMMA,              // ,
            SEMICOLON,          // ;
            COLON,              // :
            POINT,              // .
            ARROW,              // ^
            LEFT_BRACKET,       // (
            RIGHT_BRACKET,      // )
            LEFT_SQ_BRACKET,    // [
            RIGHT_SQ_BRACKET,   // ]
            LEFT_F_BRACKET,     // {
            RIGHT_F_BRACKET,    // }
            GREATER,            // >
            LESS,               // <
            GREATER_EQ,         // >=
            LESS_EQ,            // <=
            NOT_EQ,             // <>
            PLUS,               // +
            MINUS,              // -
            LEFT_COMMENT,       // (*
            RIGHT_COMMENT,      // *)
            ASSIGN,             // :=
            TWO_POINTS,         // ..
            REAL,
            INT,
            CHAR,
            STRING,
            BOOLEAN,
            IDENT,
            FLOAT_CONST,
            INT_CONST,
            STRING_CONST,
            CHAR_CONST,
            BOOLEAN_CONST,
            CASESY,
            ELSESY,
            FILESY,
            GOTOSY,
            THENSY,
            TYPESY,
            UNTILSY,
            DOSY,
            WITHSY,
            IFSY,
            PROCEDURESY,
            FUNCTIONSY,
            PROGRAMSY,
            REPEATSY,
            RECORDSY,
            PACKEDSY,
            DOWNTOSY,
            LABELSY,
            CONSTSY,
            ARRAYSY,
            WHILESY,
            BEGINSY,
            SETSY,
            NILSY,
            MODSY,
            FORSY,
            NOTSY,
            ANDSY,
            DIVSY,
            VARSY,
            ENDSY,
            TOSY,
            INSY,
            ORSY,
            OFSY,
        }

        public enum ErrorTypes
        {
            Lex,
            Synt,
            Sem
        }
        public enum ErrorCodes
        {
            ERROR,
            NO_ENDING_DOUBLE_QUOTE,
            NO_ENDING_SINGLE_QUOTE,
            CONST_ERROR,
            SYNTAX_ERROR,
            SEMANTIC_ERROR,
            NAME_EXISTS,
            TYPE_EXISTS,
            UNKNOWN_TYPE,
            UNKNOWN_IDENT,
            WRONG_TYPE,
        }

        public static readonly Dictionary<ErrorCodes, string> descriptions = new Dictionary<ErrorCodes, string>()
        {
            { ErrorCodes.ERROR, "Ошибка" },
            { ErrorCodes.NO_ENDING_DOUBLE_QUOTE, "Не хватает закрывающего символа \"" },
            { ErrorCodes.NO_ENDING_SINGLE_QUOTE, "Не хватает закрывающего символа '" },
            { ErrorCodes.CONST_ERROR, "Ошибочная константа" },
            { ErrorCodes.SYNTAX_ERROR, "Ожидалось " },
            { ErrorCodes.SEMANTIC_ERROR, "Семантическая ошибка " },
            { ErrorCodes.NAME_EXISTS, "Такое имя уже существует " },
            { ErrorCodes.TYPE_EXISTS, "Такой тип уже существует " },
            { ErrorCodes.UNKNOWN_TYPE, "Неизвестный тип " },
            { ErrorCodes.UNKNOWN_IDENT, "Неизвестный идентификатор " },
            { ErrorCodes.WRONG_TYPE, "Неверный тип " },
        };

        public static readonly char[] delimeters = new char[] { '\0', ',', '\'', '\"', '.', '(', ')', '[', ']', ';', ':', '=', '<', '>', '+', '-', '*', '/', '&' };

        public static readonly Dictionary<string, Tags> keywords = new Dictionary<string, Tags>()
        {
            {"if", Tags.IFSY },
            {"do", Tags.DOSY },
            {"of", Tags.OFSY },
            {"or", Tags.ORSY },
            {"in", Tags.INSY },
            {"to", Tags.TOSY },
            {"end", Tags.ENDSY },
            {"var", Tags.VARSY },
            {"div", Tags.DIVSY },
            {"and", Tags.ANDSY },
            {"not", Tags.NOTSY },
            {"for", Tags.FORSY },
            {"mod", Tags.MODSY },
            {"nil", Tags.NILSY },
            {"set", Tags.SETSY },
            {"then", Tags.THENSY },
            {"else", Tags.ELSESY },
            {"case", Tags.CASESY },
            {"goto", Tags.GOTOSY },
            {"type", Tags.TYPESY },
            {"with", Tags.WITHSY },
            {"begin", Tags.BEGINSY },
            {"while", Tags.WHILESY },
            {"array", Tags.ARRAYSY },
            {"const", Tags.CONSTSY },
            {"label", Tags.LABELSY },
            {"until", Tags.UNTILSY },
            {"downto", Tags.DOWNTOSY },
            {"packed", Tags.PACKEDSY },
            {"record", Tags.RECORDSY },
            {"repeat", Tags.REPEATSY },
            {"program", Tags.PROGRAMSY },
            {"function", Tags.FUNCTIONSY },
            {"procedure", Tags.PROCEDURESY },
        };

        public static readonly Tags[] plusMinus = new Tags[] { Tags.PLUS, Tags.MINUS };
        public static readonly Tags[] starSlash = new Tags[] { Tags.STAR, Tags.SLASH };
        public static readonly Tags[] divMod = new Tags[] { Tags.DIVSY, Tags.MODSY };
        public static readonly Tags[] booleanOperators = new Tags[] { Tags.ANDSY, Tags.ORSY, Tags.NOTSY };
        public static readonly Tags[] booleanOperatorsExceptNot = new Tags[] { Tags.ANDSY, Tags.ORSY };
        public static readonly Tags[] compareOperators = new Tags[] { Tags.EQUAL, Tags.GREATER, Tags.GREATER_EQ, Tags.LESS, Tags.LESS_EQ };

        public enum Places
        {
            PROGRAM,
            VAR_BLOCK,
            VAR_DECLARATION,
            TYPE_BLOCK,
            TYPE_DECLARATION,
            TYPE,
            IF,
            WHILE,
            FOR,
            BEGIN_END,
            BEGIN_END_SMALL,
            STATEMENT,
            EXPRESSION,
        }

        public static readonly Dictionary<Places, Tags[]> followers = new Dictionary<Places, Tags[]>()
        {
            { Places.PROGRAM, new Tags[] { Tags.POINT } },
            { Places.VAR_BLOCK, new Tags[] { Tags.IDENT, Tags.TYPESY, Tags.BEGINSY, Tags.SEMICOLON } },
            { Places.VAR_DECLARATION, new Tags[] { Tags.IDENT, Tags.TYPESY, Tags.BEGINSY, Tags.SEMICOLON } },
            { Places.TYPE_BLOCK, new Tags[] { Tags.IDENT, Tags.VARSY, Tags.BEGINSY, Tags.SEMICOLON } },
            { Places.TYPE_DECLARATION, new Tags[] { Tags.IDENT, Tags.VARSY, Tags.BEGINSY, Tags.SEMICOLON } },
            { Places.TYPE, new Tags[] { Tags.IDENT, Tags.VARSY, Tags.BEGINSY, Tags.SEMICOLON } },
            { Places.IF, new Tags[] { Tags.IDENT, Tags.ENDSY } },
            { Places.WHILE, new Tags[] { Tags.IDENT, Tags.ENDSY } },
            { Places.FOR, new Tags[] { Tags.IDENT, Tags.SEMICOLON } },
            { Places.BEGIN_END, new Tags[] { Tags.IDENT, Tags.SEMICOLON } },
            { Places.BEGIN_END_SMALL, new Tags[] { Tags.IDENT, Tags.SEMICOLON } },
            { Places.STATEMENT, new Tags[] { Tags.IDENT, Tags.SEMICOLON } },
            { Places.EXPRESSION, new Tags[] { Tags.IDENT, Tags.SEMICOLON } },
        };

        public static readonly Dictionary<Tags, Tags> pairs = new Dictionary<Tags, Tags>()
        {
            {Tags.BEGINSY, Tags.ENDSY },
            {Tags.LEFT_BRACKET, Tags.RIGHT_BRACKET },
            {Tags.LEFT_SQ_BRACKET, Tags.RIGHT_SQ_BRACKET },
            {Tags.LEFT_F_BRACKET, Tags.RIGHT_F_BRACKET },
        };

        public enum Types
        {
            INT,
            FLOAT,
            CHAR,
            STRING,
            BOOL,
            VOID
        }

        public static readonly Dictionary<Types, string> typeNames = new Dictionary<Types, string>()
        {
            { Types.INT, "integer" },
            { Types.FLOAT, "real" },
            { Types.CHAR, "char" },
            { Types.STRING, "string" },
            { Types.BOOL, "boolean" },
            { Types.VOID, "" },
        };

        public static readonly Dictionary<string, Types> typeEnums = new Dictionary<string, Types>()
        {
            { "integer", Types.INT },
            { "real", Types.FLOAT },
            { "char", Types.CHAR },
            { "string", Types.STRING },
            { "boolean", Types.BOOL },
        };
    }
}
