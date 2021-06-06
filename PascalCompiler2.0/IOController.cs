using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PascalCompiler2
{
    class IOController : IDisposable
    {
        string path;
        StreamReader sr;
        StreamWriter sw;

        public IOController(string path)
        {
            this.path = "../../../" + path;
            sr = new StreamReader(this.path);
            sw = new StreamWriter(this.path + ".txt");
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            sr.Close();
            sw.Close();
        }

        public string GetAllLines()
        {
            return sr.ReadToEnd();
        }

        public string GetLine()
        {
            return sr.ReadLine();
        }

        public void WriteLine(string text = "")
        {
            sw.WriteLine(text);
        }

        public void Write(string text = "")
        {
            sw.Write(text);
        }
    }
}
