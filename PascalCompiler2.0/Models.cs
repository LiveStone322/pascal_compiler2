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
        }

        public static readonly Dictionary<ErrorCodes, string> descriptions = new Dictionary<ErrorCodes, string>()
        {
            { ErrorCodes.ERROR, "Ошибка" },
            { ErrorCodes.NO_ENDING_DOUBLE_QUOTE, "Не хватает закрывающего символа \"" },
            { ErrorCodes.NO_ENDING_SINGLE_QUOTE, "Не хватает закрывающего символа '" },
            { ErrorCodes.CONST_ERROR, "Ошибочная константа" },
            { ErrorCodes.SYNTAX_ERROR, "Ожидалось " },
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
    }
}
