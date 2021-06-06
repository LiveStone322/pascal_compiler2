using PascalCompiler2.Helpers;

namespace PascalCompiler2
{
    class CodeController
    {
        public CurCharHelper curCharHelper = new CurCharHelper();
        public PositionHelper posHelper = new PositionHelper();
        public string curLine = "";

        private IOController io;

        public CodeController(IOController io)
        {
            this.io = io;
            Init();
        }

        private void Init()
        {
            ReadLine();
            posHelper.x = 0;
            curCharHelper.SetCurNext(GetChar(false), GetChar());
        }

        private char GetChar(bool offset = true)
        {
            if (offset && (posHelper.x + 1 >= curLine.Length))
            {
                return '\0';
            }
            return curLine[posHelper.x + (offset ? 1 : 0)];
        }

        private bool ReadLine()
        {
            do
            {
                curLine = io.GetLine();
                if (curLine != null)
                {
                    if (curLine.Length == 0) continue;
                    posHelper.length = curLine.Length;
                    return true;
                }
                else return false;
            }
            while (true);
        }

        public char GetCurChar()
        {
            return curCharHelper.cur;
        }

        public char GetNextChar()
        {
            return curCharHelper.next;
        }

        public bool SkipChars(int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (!NextChar()) return false;
            }
            return true;
        }

        public bool NextChar()
        {
            // get next position
            // if not EOL
            if (posHelper.Next())
            {
                curCharHelper.SetNext(GetChar());
                return true;
            }
            // if EOL and not EOF
            if (ReadLine())
            {
                curCharHelper.SetNext(GetChar(false));

                if (!posHelper.isEOL())
                {
                    curCharHelper.SetNext(GetChar());
                }
                else
                {
                    curCharHelper.SetNext('\0');
                }
                return true;
            }
            // if EOL and EOF
            return false;
        }
    }
}
