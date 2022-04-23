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
    public partial class SeedDialog : Form
    {
        public event ApplyEventHandler Apply;

        public SeedDialog()
        {
            InitializeComponent();
        }

        public int MyInteger { get; set; }
        public string MyString { get; set; }

        private void OK_Click(object sender, EventArgs e)
        {
            // Publish the event if it is not null
            // and pass the information with the custom
            // event arguements class.
            if (Apply != null) Apply(this, new ApplyEventArgs(this.MyInteger, this.MyString));
            MyInteger = (int)seedUpDown.Value;
           
        }

        private void Randomizer_Click(object sender, EventArgs e)
        {
            Random seeder = new Random((int)DateTime.Now.Ticks);
            int num = 0;
            num = seeder.Next();
            seedUpDown.Value = num;
        }
    }
}
