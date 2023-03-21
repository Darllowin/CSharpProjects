using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms_Zmeika_
{
    public partial class Form1 : Form
    {
        string path = "records.txt";
        int rI, rJ;
        PictureBox fruit;
        PictureBox[] snake = new PictureBox[400];
        Label labelScore;
        Random rnd = new Random();
        int dirX, dirY;
        int width = 900;
        int height = 800;
        int sizeOfSides = 40;
        int BestScore;
        int score;
        byte endg = 0;
        public Form1()
        {
            InitializeComponent();
            BestScore = Properties.Settings.Default.bestrecords;
            Width = width;
            Height = height;
            dirX = 1;
            dirY = 0;
            labelScore = new Label();          
            labelScore.Text = "Score: 0";            
            labelScore.Location = new Point(810, 10);
            Controls.Add(labelScore);
            
            fruit = new PictureBox();
            fruit.BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            fruit.Size = new Size(sizeOfSides, sizeOfSides);
            generateMap();
            generateFruit();
            generateSnake();
            timer1.Tick += new EventHandler(update);
            timer1.Interval = 200;
            timer1.Start();
            KeyDown += new KeyEventHandler(OKP);
        }
        void generateFruit()
        {
            Random r = new Random();
            rI = r.Next(0, height - sizeOfSides);
            int tempI = rI % sizeOfSides;
            rI -= tempI;
            rJ = r.Next(0, height - sizeOfSides);
            int tempJ = rJ % sizeOfSides;
            rJ -= tempJ;
            rI++;
            rJ++;
            fruit.Location = new Point(rI, rJ);
            Controls.Add(fruit);
            fruit.BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        }
        void generateSnake()
        {
            snake[0] = new PictureBox();
            snake[0].Location = new Point(201, 201);
            snake[0].Size = new Size(sizeOfSides - 1, sizeOfSides - 1);
            snake[0].BackColor = Color.Red;
            Controls.Add(snake[0]);
        }
        void checkBorders()
        {
            if (snake[0].Location.X < 0)
            {
                for (int i = 1; i <= score; i++)
                {
                    Controls.Remove(snake[i]);
                }
                BestRecords();
                end();
                labelScore.Text = "Score: " + score;
                dirX = 1;
            }
            if (snake[0].Location.X > height)
            {
                for (int i = 1; i <= score; i++)
                {
                    Controls.Remove(snake[i]);
                }
                BestRecords();
                end();
                labelScore.Text = "Score: " + score;
                dirX = -1;
            }
            if (snake[0].Location.Y < 0)
            {
                for (int i = 1; i <= score; i++)
                {
                    Controls.Remove(snake[i]);
                }
                BestRecords();
                end();
                labelScore.Text = "Score: " + score;
                dirY = 1;
            }
            if (snake[0].Location.Y > height)
            {
                for (int i = 1; i <= score; i++)
                {
                    Controls.Remove(snake[i]);
                }
                BestRecords();
                end();
                labelScore.Text = "Score: " + score;
                dirY = -1;
            }
        }
        void end()
        {
            if (endg == 0)
            {
                timer1.Stop();
                endg = 1;
                string message = "Вы проиграли ваш счёт: " + score;
                score = 0;
                string caption = "Продолжить?";
                if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    endg = 0;                    
                    snake[0].Location = new Point(201, 201);
                    timer1.Start();
                }
                else
                {
                    Application.Exit();
                }
            }                  
        }
        void BestRecords()
        {
            if (score > BestScore)
            {
                BestScore = score;
                Properties.Settings.Default.bestrecords = BestScore;
                Properties.Settings.Default.Save();
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    writer.WriteLineAsync("Рекорд: " + Convert.ToString(Properties.Settings.Default.bestrecords));
                }
            }
        }
        void eatFrute()
        {
            if (snake[0].Location.X == rI && snake[0].Location.Y == rJ)
            {
                labelScore.Text = "Score: " + ++score;
                snake[score] = new PictureBox();
                snake[score].Location = new Point(snake[score - 1].Location.X + 40 * dirX, snake[score - 1].Location.Y - 40 * dirY);
                snake[score].Size = new Size(sizeOfSides - 1, sizeOfSides - 1);
                snake[score].BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                Controls.Add(snake[score]);
                generateFruit();
                timer1.Interval -= 5;
            }
        }
        void eatItself()
        {
            for (int i = 1; i <= score; i++)
            {
                if (snake[0].Location == snake[i].Location)
                {
                    Controls.Remove(snake[i]);
                    Controls.Remove(snake[score]);
                    end();
                }
            }
        }
        void generateMap()
        {
            for (int i = 0; i < width / sizeOfSides; i++)
            {
                PictureBox pic = new PictureBox();
                pic.BackColor = Color.Blue;
                pic.Location = new Point(0, sizeOfSides * i);
                pic.Size = new Size(width - 100, 1);
                Controls.Add(pic);
            }
            for (int i = 0; i <= height / sizeOfSides; i++)
            {
                PictureBox pic = new PictureBox();
                pic.BackColor = Color.Blue;
                pic.Location = new Point(sizeOfSides * i, 0);
                pic.Size = new Size(1, width);
                Controls.Add(pic);
            }
        }
        void moveSnake()
        {
            for (int i = score; i >= 1; i--)
            {
                snake[i].Location = snake[i - 1].Location;
            }
            snake[0].Location = new Point(snake[0].Location.X + dirX * (sizeOfSides), snake[0].Location.Y + dirY * (sizeOfSides));              
            eatItself();
        }
        void update(object obj, EventArgs eventsArgs)
        {
            checkBorders();
            eatFrute();
            moveSnake();           
        }
        void OKP(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "Right":
                    dirX = 1;
                    dirY = 0;
                    break;
                case "Left":
                    dirX = -1;
                    dirY = 0;
                    break;
                case "Up":
                    dirY = -1;
                    dirX = 0;
                    break;
                case "Down":
                    dirY = 1;
                    dirX = 0;
                    break;
                default:
                    timer1.Stop();
                    MessageBox.Show("Пауза! Ваши очки: " + score + "\n Чтобы продолжить нажмите 'ОК'!");
                    timer1.Start();
                    break;
            }
        }       
    }
}
