namespace PascalCompiler2.Helpers
{
    class CurCharHelper
    {
        public char cur;
        public char next;

        public void SetNext(char next)
        {
            cur = this.next;
            this.next = next;
        }

        public void SetCurNext(char cur, char next)
        {
            this.cur = cur;
            this.next = next;
        }
    }
}
