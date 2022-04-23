using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_GOL
{
    public class ApplyEventArgs : EventArgs
    {
        int myInteger;
        string myString;
        public int MyInteger
        {
            get { return myInteger; }
            set { myInteger = value; }
        }
        public string MyString
        {
            get { return myString; }
            set { myString = value; }
        }
        public ApplyEventArgs(int myInteger, string myString)
        {
            this.myInteger = myInteger;
            this.myString = myString;
        }

    }
    public delegate void ApplyEventHandler(object sender, ApplyEventArgs e);
}
