using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler2
{
    class Token
    {
        public Models.Tags tag;
        public string val;
        public int lineIndex;
        public int charIndex;
        
        public Token(Models.Tags tag, string val, int lineIndex, int charIndex)
        {
            this.tag = tag;
            this.val = val;
            this.lineIndex = lineIndex;
            this.charIndex = charIndex;
        }
    }
}
