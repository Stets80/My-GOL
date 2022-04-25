﻿using System;
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
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[20, 20];
        int[,] numuniverse = new int[20, 20];

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
        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 1; // milliseconds
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
                        CountNeighborsFinite(i, j);
                    }
                }
            }
            // rules for createing new cells and dying cells
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    if (universe[i, j] == true && numuniverse[i, j] < 2)
                    {
                        universe[i, j] = false;
                    }
                    if (universe[i, j] == true && numuniverse[i, j] > 3)
                    {
                        universe[i, j] = false;
                    }
                    if (universe[i, j] == true && numuniverse[i, j] == 2 || numuniverse[i, j] == 3)
                    {
                        universe[i, j] = true;
                    }
                    if (universe[i, j] == false && numuniverse[i, j] == 3)
                    {
                        universe[i, j] = true;
                    }
                    graphicsPanel1.Invalidate();
                }
            }

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
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // A brush for filling dead cells
            Brush deadcellBrush = new SolidBrush(deadcellcolor);

            // a brush and color for the hud
            Color hudcolor = Color.FromArgb(20, 255, 0, 0);
            Brush hudbrush = new SolidBrush(hudcolor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Fill the cell with a brush if dead
                    if (universe[x, y] == false)
                    {
                        e.Graphics.FillRectangle(deadcellBrush, cellRect);
                    }

                    // paint numbers in cells
                    Font font = new Font("Arial", 10f);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;



                    Rectangle rect = new Rectangle(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    int neighbors = 0;
                    if (universetype == true)
                    {
                        neighbors = CountNeighborsToroidal(x, y);

                    }
                    else
                    {
                        neighbors = CountNeighborsFinite(x, y);
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
                //hud
                if (hudtoggle == true)
                {
                    StringFormat hudstringFormat = new StringFormat();
                    hudstringFormat.Alignment = StringAlignment.Near;
                    hudstringFormat.LineAlignment = StringAlignment.Far;
                    Rectangle hudrect = new Rectangle(universe.GetLength(0), universe.GetLength(1), graphicsPanel1.ClientSize.Width, graphicsPanel1.ClientSize.Height - 40);
                    Font font1 = new Font("Arial", 20f);

                    e.Graphics.DrawString("Generations = " + generations.ToString() + "\nseed = " + seed.ToString() + "\nCell Count = " + alive.ToString(), font1, hudbrush, hudrect, hudstringFormat);

                }

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
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

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
            PasuetoolStripButton.Enabled = true;
        }
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            startToolStripMenuItem.Enabled = true;
            PlaytoolStripButton.Enabled = true;
            PasuetoolStripButton.Enabled = false;
        }
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        private void PlaytoolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
            PlaytoolStripButton.Enabled = false;
            PasuetoolStripButton.Enabled = true;
            startToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;
        }
        private void PasuetoolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            PlaytoolStripButton.Enabled = true;
            PasuetoolStripButton.Enabled = false;
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
            PasuetoolStripButton.Enabled = false;
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
            PasuetoolStripButton.Enabled = false;
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
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabelseed.Text = "seed = " + seed.ToString();
            toolStripStatusLabelcellcount.Text = "Cell Count = " + alive.ToString();
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
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.livingcell = cellColor;
            Properties.Settings.Default.gridlinecolor = gridColor;
            Properties.Settings.Default.deadcell = deadcellcolor;
            Properties.Settings.Default.UniverseType = universetype;
            Properties.Settings.Default.GridToggle = gridtoggle;
            Properties.Settings.Default.NeighborCountToggle = neighborcounttoggle;
            Properties.Settings.Default.HudToggle = hudtoggle;
            Properties.Settings.Default.Save();
        }
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            cellColor = Properties.Settings.Default.livingcell;
            gridColor = Properties.Settings.Default.gridlinecolor;
            deadcellcolor = Properties.Settings.Default.deadcell;
            universetype = Properties.Settings.Default.UniverseType;
            gridtoggle = Properties.Settings.Default.GridToggle;
            neighborcounttoggle = Properties.Settings.Default.NeighborCountToggle;
            hudtoggle = Properties.Settings.Default.HudToggle;
            graphicsPanel1.Invalidate();
        }
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            cellColor = Properties.Settings.Default.livingcell;
            gridColor = Properties.Settings.Default.gridlinecolor;
            deadcellcolor = Properties.Settings.Default.deadcell;
            universetype = Properties.Settings.Default.UniverseType;
            gridtoggle = Properties.Settings.Default.GridToggle;
            neighborcounttoggle = Properties.Settings.Default.NeighborCountToggle;
            hudtoggle = Properties.Settings.Default.HudToggle;
            graphicsPanel1.Invalidate();
        }
    }
}
