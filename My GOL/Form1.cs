using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace My_GOL
{
    public partial class Form1 : Form
    {

        // The universe array
        bool[,] universe = new bool[Properties.Settings.Default.Rows, Properties.Settings.Default.Coloumns];
        bool[,] scratchPad = new bool[Properties.Settings.Default.Rows, Properties.Settings.Default.Coloumns];
        int[,] numuniverse = new int[Properties.Settings.Default.Rows, Properties.Settings.Default.Coloumns];

        //universe defaults to torodial universetype true/false = torodial/finite
        bool universetype = Properties.Settings.Default.UniverseType;
        bool gridtoggle = Properties.Settings.Default.GridToggle;
        bool neighborcounttoggle = Properties.Settings.Default.NeighborCountToggle;
        bool hudtoggle = Properties.Settings.Default.HudToggle;

        // Drawing colors
        Color gridColor = Properties.Settings.Default.gridlinecolor;
        Color cellColor = Properties.Settings.Default.livingcell;
        Color deadcellcolor = Properties.Settings.Default.deadcell;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int alive = 0;
        int generations = 0;

        // seed for randimzer
        int seed = 0;
        int time = Properties.Settings.Default.Interval;
        int row = Properties.Settings.Default.Rows;
        int coloumn = Properties.Settings.Default.Coloumns;
        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = time; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running

            menuchecker();

            statUpdate();
        }
        private void NextGeneration()
        {
            // Calculate the next generation of cells
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    if (universetype == true)
                    {
                        numuniverse[i, j] = CountNeighborsToroidal(i, j);
                    }
                    else
                    {
                        numuniverse[i, j] = CountNeighborsFinite(i, j);
                    }
                }
            }
            // rules for createing new cells and dying cells
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    scratchPad[i, j] = false;

                    if (universe[i, j] == true && numuniverse[i, j] < 2)
                    {
                        scratchPad[i, j] = false;
                    }
                    if (universe[i, j] == true && numuniverse[i, j] > 3)
                    {
                        scratchPad[i, j] = false;
                    }
                    if (universe[i, j] == true && numuniverse[i, j] == 2 || numuniverse[i, j] == 3)
                    {
                        scratchPad[i, j] = true;
                    }
                    if (universe[i, j] == false && numuniverse[i, j] == 3)
                    {
                        scratchPad[i, j] = true;
                    }

                    graphicsPanel1.Invalidate();
                }
            }

            //write scratchpad to universe
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations
            statUpdate();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // The event called by the timer every Interval milliseconds.
            NextGeneration();
        }
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // A brush for filling dead cells
            Brush deadcellBrush = new SolidBrush(deadcellcolor);

            // a brush and color for the hud
            Color hudcolor = Color.FromArgb(130, 255, 0, 0);
            Brush hudbrush = new SolidBrush(hudcolor);

            // Iterate through the universe in the y, top to bottom
            for (float y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (float x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[(int)x, (int)y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Fill the cell with a brush if dead
                    if (universe[(int)x, (int)y] == false)
                    {
                        e.Graphics.FillRectangle(deadcellBrush, cellRect);
                    }

                    // paint numbers in cells
                    Font font = new Font("Arial", 10f);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    RectangleF rect = new RectangleF(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    int neighbors = 0;
                    if (universetype == true)
                    {
                        neighbors = CountNeighborsToroidal((int)x, (int)y);
                    }
                    else
                    {
                        neighbors = CountNeighborsFinite((int)x, (int)y);
                    }
                    if (neighborcounttoggle == true)
                    {
                        if (neighbors != 0)
                        {
                            if (neighbors == 3)
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, rect, stringFormat);
                            }
                            else
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, rect, stringFormat);
                            }
                        }
                    }
                    // Outline the cell with a pen
                    if (gridtoggle == true)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                }
            }
            //hud
            if (hudtoggle == true)
            {
                StringFormat hudstringFormat = new StringFormat();
                hudstringFormat.Alignment = StringAlignment.Near;
                hudstringFormat.LineAlignment = StringAlignment.Far;
                RectangleF hudrect = new RectangleF(universe.GetLength(0), universe.GetLength(1), graphicsPanel1.ClientSize.Width, graphicsPanel1.ClientSize.Height - 40);
                Font hudfont = new Font("Arial", 15f);

                e.Graphics.DrawString("Generations = " + generations.ToString() + "\nseed = " + seed.ToString() + "\nCell Count = " + alive.ToString() + "\nInterval = " + time.ToString(), hudfont, hudbrush, hudrect, hudstringFormat);
            }
            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            deadcellBrush.Dispose();
            hudbrush.Dispose();
        }
        private int CountNeighborsToroidal(int x, int y)
        {
            //counts the neighbors live cells in a infinite universe
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if ((xOffset == 0) && (yOffset == 0))
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        private int CountNeighborsFinite(int x, int y)
        {
            //counts the neighbors live cells in a finite universe
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if ((xOffset == 0) && (yOffset == 0))
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                statUpdate();
                graphicsPanel1.Invalidate();
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
            startToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;
            PlaytoolStripButton.Enabled = false;
            pausetoolStripButton.Enabled = true;
        }
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            startToolStripMenuItem.Enabled = true;
            PlaytoolStripButton.Enabled = true;
            pausetoolStripButton.Enabled = false;
        }
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        private void PlaytoolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
            PlaytoolStripButton.Enabled = false;
            pausetoolStripButton.Enabled = true;
            startToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;
        }
        private void PasuetoolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            PlaytoolStripButton.Enabled = true;
            pausetoolStripButton.Enabled = false;
            startToolStripMenuItem.Enabled = true;
            pauseToolStripMenuItem.Enabled = false;
        }
        private void NexttoolStripButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hudtoggle == true)
            {
                hudtoggle = false;
                hUDToolStripMenuItem.Checked = false;
                graphicsPanel1.Invalidate();
            }
            else if (hudtoggle == false)
            {
                hudtoggle = true;
                hUDToolStripMenuItem.Checked = true;
                graphicsPanel1.Invalidate();
            }
        }
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (neighborcounttoggle == true)
            {
                neighborcounttoggle = false;
                neighborCountToolStripMenuItem.Checked = false;
                graphicsPanel1.Invalidate();
            }
            else if (neighborcounttoggle == false)
            {
                neighborcounttoggle = true;
                neighborCountToolStripMenuItem.Checked = true;
                graphicsPanel1.Invalidate();
            }
        }
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridtoggle == true)
            {
                gridtoggle = false;
                gridToolStripMenuItem.Checked = false;
                graphicsPanel1.Invalidate();
            }
            else if (gridtoggle == false)
            {
                gridtoggle = true;
                gridToolStripMenuItem.Checked = true;
                graphicsPanel1.Invalidate();
            }
        }
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universetype = true;
            toroidalToolStripMenuItem.Checked = true;
            toroidalToolStripMenuItem1.Checked = true;
            finiteToolStripMenuItem.Checked = false;
            finiteToolStripMenuItem1.Checked = false;
            graphicsPanel1.Invalidate();
        }
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universetype = false;
            finiteToolStripMenuItem.Checked = true;
            finiteToolStripMenuItem1.Checked = true;
            toroidalToolStripMenuItem.Checked = false;
            toroidalToolStripMenuItem1.Checked = false;
            graphicsPanel1.Invalidate();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            startToolStripMenuItem.Enabled = true;
            PlaytoolStripButton.Enabled = true;
            pausetoolStripButton.Enabled = false;
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    universe[i, j] = false;
                }
            }
            generations = 0;
            statUpdate();
            graphicsPanel1.Invalidate();
        }
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            startToolStripMenuItem.Enabled = true;
            PlaytoolStripButton.Enabled = true;
            pausetoolStripButton.Enabled = false;
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    universe[i, j] = false;
                }
            }
            generations = 0;
            statUpdate();
            graphicsPanel1.Invalidate();
        }
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedDialog dlg = new SeedDialog();
            dlg.setseed(seed);

            if (DialogResult.OK == dlg.ShowDialog())
            {
                seed = dlg.getseed();
                randomizer(seed);
            }
        }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            options dlg = new options();
            dlg.settime(time);
            dlg.setwidth(row);
            dlg.setheight(coloumn);

            if (DialogResult.OK == dlg.ShowDialog())
            {
                time = dlg.gettime();
                timer.Interval = dlg.gettime();
                row = dlg.getwidth();
                coloumn = dlg.getheight();
                universe = new bool[row, coloumn];
                scratchPad = new bool[row, coloumn];
                numuniverse = new int[row, coloumn];
                timer.Enabled = false;
                pauseToolStripMenuItem.Enabled = false;
                startToolStripMenuItem.Enabled = true;
                PlaytoolStripButton.Enabled = true;
                pausetoolStripButton.Enabled = false;
                generations = 0;
                statUpdate();
            }
        }
        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            randomizer(seed);
        }
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timeRandomizer();
        }
        public void randomizer(int Seed)
        {
            //randomizer
            Random seeder = new Random(Seed);
            int num = 0;

            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    num = (seeder.Next(0, 2));
                    if (num == 0)
                    {
                        universe[i, j] = true;
                    }
                    else
                    {
                        universe[i, j] = false;
                    }
                }
            }
            statUpdate();
            graphicsPanel1.Invalidate();
        }
        public void timeRandomizer()
        {
            seed = (int)DateTime.Now.Ticks;
            Random seeder = new Random(seed);
            int num = 0;
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    num = seeder.Next(0, 2);
                    if (num == 0)
                    {
                        universe[i, j] = true;
                    }
                    else
                    {
                        universe[i, j] = false;
                    }
                }
            }
            statUpdate();
            graphicsPanel1.Invalidate();
        }
        public void statUpdate()
        {
            alive = 0;
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    if (universe[i, j] == true)
                    {
                        alive++;
                    }
                }
            }
            timer.Interval = time;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabelseed.Text = "seed = " + seed.ToString();
            toolStripStatusLabelcellcount.Text = "Cell Count = " + alive.ToString();
            toolStripStatusLabelinterval.Text = "interval = " + time.ToString();
            graphicsPanel1.Invalidate();
        }
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog deadcell = new ColorDialog();
            deadcell.Color = deadcellcolor;
            if (DialogResult.OK == deadcell.ShowDialog())
            {
                deadcellcolor = deadcell.Color;
                graphicsPanel1.Invalidate();
            }
        }
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog livingcell = new ColorDialog();
            livingcell.Color = cellColor;
            if (DialogResult.OK == livingcell.ShowDialog())
            {
                cellColor = livingcell.Color;
                graphicsPanel1.Invalidate();
            }
        }
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog newgridcolor = new ColorDialog();
            newgridcolor.Color = gridColor;
            if (DialogResult.OK == newgridcolor.ShowDialog())
            {
                gridColor = newgridcolor.Color;
                graphicsPanel1.Invalidate();
            }
        }
        private void menuchecker()
        {
            if (universetype == true)
            {
                toroidalToolStripMenuItem.Checked = true;
                toroidalToolStripMenuItem1.Checked = true;
            }
            else if (universetype == false)
            {
                finiteToolStripMenuItem.Checked = true;
                finiteToolStripMenuItem1.Checked = true;
            }
            if (gridtoggle == true)
            {
                gridToolStripMenuItem.Checked = true;
                gridToolStripMenuItem1.Checked = true;

            }
            if (neighborcounttoggle == true)
            {
                neighborCountToolStripMenuItem.Checked = true;
                neighborCountToolStripMenuItem1.Checked = true;
            }
            if (hudtoggle == true)
            {
                hUDToolStripMenuItem.Checked = true;
                hUDToolStripMenuItem1.Checked = true;
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!" + DateTime.Now.ToString() + " date when file was saved.");


                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y] == true)
                        {
                            currentRow += 'O';
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else if (universe[x, y] == false)
                        {
                            currentRow += '.';
                        }
                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }
                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.

                    if (!row.StartsWith("!"))
                    {
                        maxHeight++;
                    }
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    if (!row.StartsWith("!"))
                    {
                        maxWidth = row.Count();
                    }
                }
                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                timer.Enabled = false;
                pauseToolStripMenuItem.Enabled = false;
                startToolStripMenuItem.Enabled = true;
                PlaytoolStripButton.Enabled = true;
                pausetoolStripButton.Enabled = false;
                generations = 0;
                row = maxWidth;
                coloumn = maxHeight;
                universe = new bool[row, coloumn];
                scratchPad = new bool[row, coloumn];
                numuniverse = new int[row, coloumn];
                statUpdate();


                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                int yPos = 0;
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    if (!row.StartsWith("!"))
                    {
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if (row[xPos] == 'O')
                            {
                                universe[xPos, yPos] = true;
                                graphicsPanel1.Invalidate();
                            }

                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                            if (row[xPos] == '.')
                            {
                                universe[xPos, yPos] = false;
                                graphicsPanel1.Invalidate();
                            }
                        }
                    }
                    yPos++;
                }
                // Close the file.
                reader.Close();
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.livingcell = cellColor;
            Properties.Settings.Default.gridlinecolor = gridColor;
            Properties.Settings.Default.deadcell = deadcellcolor;
            Properties.Settings.Default.UniverseType = universetype;
            Properties.Settings.Default.GridToggle = gridtoggle;
            Properties.Settings.Default.NeighborCountToggle = neighborcounttoggle;
            Properties.Settings.Default.HudToggle = hudtoggle;
            Properties.Settings.Default.Interval = time;
            Properties.Settings.Default.Rows = row;
            Properties.Settings.Default.Coloumns = coloumn;
            Properties.Settings.Default.Save();
        }
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statUpdate();
            Properties.Settings.Default.Reset();
            cellColor = Properties.Settings.Default.livingcell;
            gridColor = Properties.Settings.Default.gridlinecolor;
            deadcellcolor = Properties.Settings.Default.deadcell;
            universetype = Properties.Settings.Default.UniverseType;
            gridtoggle = Properties.Settings.Default.GridToggle;
            neighborcounttoggle = Properties.Settings.Default.NeighborCountToggle;
            hudtoggle = Properties.Settings.Default.HudToggle;
            time = Properties.Settings.Default.Interval;
            row = Properties.Settings.Default.Rows;
            coloumn = Properties.Settings.Default.Coloumns;
            universe = new bool[row, coloumn];
            scratchPad = new bool[row, coloumn];
            numuniverse = new int[row, coloumn];
            timer.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            startToolStripMenuItem.Enabled = true;
            PlaytoolStripButton.Enabled = true;
            pausetoolStripButton.Enabled = false;
            generations = 0;
            statUpdate();
            graphicsPanel1.Invalidate();
        }
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statUpdate();
            Properties.Settings.Default.Reload();
            cellColor = Properties.Settings.Default.livingcell;
            gridColor = Properties.Settings.Default.gridlinecolor;
            deadcellcolor = Properties.Settings.Default.deadcell;
            universetype = Properties.Settings.Default.UniverseType;
            gridtoggle = Properties.Settings.Default.GridToggle;
            neighborcounttoggle = Properties.Settings.Default.NeighborCountToggle;
            hudtoggle = Properties.Settings.Default.HudToggle;
            time = Properties.Settings.Default.Interval;
            row = Properties.Settings.Default.Rows;
            coloumn = Properties.Settings.Default.Coloumns;
            universe = new bool[row, coloumn];
            scratchPad = new bool[row, coloumn];
            numuniverse = new int[row, coloumn];
            timer.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            startToolStripMenuItem.Enabled = true;
            PlaytoolStripButton.Enabled = true;
            pausetoolStripButton.Enabled = false;
            generations = 0;
            statUpdate();
            graphicsPanel1.Invalidate();
        }
    }
}
