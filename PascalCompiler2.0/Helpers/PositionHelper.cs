namespace PascalCompiler2.Helpers
{
    class PositionHelper
    {
        public int y;
        public int x;
        public int length;

        public PositionHelper()
        {
            y = 0;
            x = 0;
            length = 0;
        }

        public PositionHelper(int y, int x, int length)
        {
            this.y = y;
            this.x = x;
            this.length = length;
        }

        public bool Next()
        {
            if (!isEOL())
            {
                x++;
                return true;
            }
            x = 0;
            y++;
            return false;
        }

        public bool isEOL()
        {
            return x + 1 >= length;
        }

        public void NextLine()
        {
            y++;
        }
    }
}
