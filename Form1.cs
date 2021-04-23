using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Сапер__лабораторная_4_
{
    public partial class Miner : Form
    {
        int width = 10;
        int height = 10;
        int offset = 30;
        int bombPercent = 5;
        bool isFirstClick = true;
        FieldButton[,] field;
        int cellsOpened = 0;
        int bombs = 0;

        int min, sec, ms;


        public Miner()
        {
            InitializeComponent();
            timer1.Interval = 500;
            label3.Visible = true;
            min = 0;
            sec = 0;
            ms = 0;
            label1.Text = "00";
            label2.Text = "00";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            field = new FieldButton[width, height];
            GenerateField();
           
        }

        public void GenerateField()
        {
            Random random = new Random();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    FieldButton newButton = new FieldButton();
                    newButton.Location = new Point(x * offset, (y+1) * offset);
                    newButton.Size = new Size(offset, offset);
                    newButton.isClickable = true;
                    if (random.Next(0, 100) <= bombPercent)
                    {
                        newButton.isBomb = true;
                        bombs++;
                    }
                    newButton.xCoord = x;
                    newButton.yCoord = y;
                    Controls.Add(newButton);
                    newButton.MouseUp += new MouseEventHandler(FieldButtonClick);   
                    field[x, y] = newButton;
                }
            }
        }

        void FieldButtonClick(object sender, MouseEventArgs e)
        {

            FieldButton clickedButton = (FieldButton)sender;
            if (e.Button == MouseButtons.Left && clickedButton.isClickable)  
            {
                if (clickedButton.isBomb)
                {
                    if (isFirstClick)
                    {
                        timer1.Start();
                        timer1.Enabled = true;
                        clickedButton.isBomb = false;
                        isFirstClick = false;
                        bombs--;
                        OpenRegion(clickedButton.xCoord, clickedButton.yCoord, clickedButton);
                    }
                    else
                    {
                        Explode();
                    }

                }
                else
                {
                    EmptyFieldButtonClick(clickedButton);
                }
                isFirstClick = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                clickedButton.isClickable = !clickedButton.isClickable;
                if (!clickedButton.isClickable)
                {
                    clickedButton.Image = Image.FromFile("C:/Users/Lev/Pictures/Saved Pictures/flagsInMiner.png");
                }
                else
                {
                    clickedButton.Text = "";
                }
            }
            CheckWin();
        }

        void Explode()
        {
            timer1.Stop();
            listBox1.Items.Add($"{min} : {sec}");
            min = 0;
            sec = 0;
            label1.Text = "00";
            label2.Text = "00";
            foreach (FieldButton button in field)
            {
                if (button.isBomb)
                {
                    button.Image = Image.FromFile("C:/Users/Lev/Pictures/Saved Pictures/tiles (4).png");
                }
            }
            MessageBox.Show("Поражение");
            Application.Restart();
        }
        void EmptyFieldButtonClick(FieldButton clickedButton)
        {

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (field[x, y] == clickedButton)
                    {
                        
                        OpenRegion(x, y, clickedButton);
                    }
                }
            }
        }

        void OpenRegion(int xCoord, int yCoord, FieldButton clickedButton)
        {
            Queue<FieldButton> queue = new Queue<FieldButton>();
            queue.Enqueue(clickedButton);
            clickedButton.wasAdded = true;
            while (queue.Count > 0)
            {
                FieldButton currentCell = queue.Dequeue();
                OpenCell(currentCell.xCoord, currentCell.yCoord, currentCell);
                cellsOpened++;
                if (CountBombsAround(currentCell.xCoord, currentCell.yCoord) == 0)
                {
                    for (int y = currentCell.yCoord - 1; y <= currentCell.yCoord + 1; y++)
                    {
                        for (int x = currentCell.xCoord - 1; x <= currentCell.xCoord + 1; x++)
                        {
                            if (x == currentCell.xCoord && y == currentCell.yCoord)//
                            {
                                continue;
                            }
                            if (x >= 0 && x < width && y < height && y >= 0)
                            {
                                if (!field[x, y].wasAdded)
                                {
                                    queue.Enqueue(field[x, y]);
                                    field[x, y].wasAdded = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        void OpenCell(int x, int y, FieldButton clickedButton)
        {
            int bombsAround = CountBombsAround(x, y);
            if (bombsAround == 0)
            {
            }
            else
            {
                clickedButton.BackColor = Color.White;
                clickedButton.Text = "" + bombsAround;
            }
            clickedButton.Enabled = false;
        }

        int CountBombsAround(int xCoord, int yCoord)
        {
            int bombsAround = 0;
            for (int x = xCoord - 1; x <= xCoord + 1; x++)
            {
                for (int y = yCoord - 1; y <= yCoord + 1; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        if (field[x, y].isBomb == true)
                        {
                            bombsAround++;
                        }
                    }
                }
            }
            return bombsAround;
        }

        void CheckWin()
        {
            int cells = width * height;
            int emptyCells = cells - bombs;
            if (cellsOpened >= emptyCells)
            {
                MessageBox.Show("Вы победили! :)");
                timer1.Stop();
                listBox1.Items.Add($"{min} : {sec}");
                min = 0;
                sec = 0;
                label1.Text = "00";
                label2.Text = "00";
            }
            
        }

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Вы точно хотите выйти из игры?", "Выход из игры", MessageBoxButtons.YesNo);
            if (DialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }           
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutProgram aboutProgram = new AboutProgram();
            aboutProgram.Show();
        }

        private void forTimer_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (label3.Visible)
            {
                if (sec < 59)
                {
                    sec++;
                    if (sec < 10)
                        label2.Text = "0" + sec.ToString();
                    else
                        label2.Text = sec.ToString();
                }
                else
                {
                    if (min < 59)
                    {
                        min++;
                        if (min < 10)
                            label1.Text = "0" + min.ToString();
                        else
                            label1.Text = min.ToString();
                        sec = 0;
                        label2.Text = "00";
                    }
                    else
                    {
                        min = 0;
                        label1.Text = "00";
                    }
                }
                label3.Visible = false;
            }
            else
            {
                label3.Visible = true;
            }
        }

        private void toolStripMenuItem55_Click(object sender, EventArgs e)
        {
            this.width = 5;
            this.height = 5;
            GenerateField();
        }
      
    }



    public class FieldButton : Button
    {
        public bool isBomb;
        public bool isClickable;
        public bool wasAdded;
        public int xCoord;
        public int yCoord;
    }
}

