using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My_GOL
{
    public partial class options : Form
    {
        public options()
        {
            InitializeComponent();

        }
        public int getwidth()
        {
            return (int)widthnumericUpDown.Value;
        }
        public void setwidth(int width)
        {
            widthnumericUpDown.Value = width;
        }
        public int getheight()
        {
            return (int)heightnumericUpDown.Value;
        }
        public void setheight(int height)
        {
            heightnumericUpDown.Value = height;
        }
        public int gettime()
        {
            return (int)intervalnumericUpDown.Value;
        }
        public void settime(int time)
        {
            intervalnumericUpDown.Value = time;
        }
    }
}
