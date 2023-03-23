using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Media;
using System.Windows.Media;

namespace WindowsForms_Zmeika_
{
    public partial class Form1 : Form
    {
        // Элементы
        PictureBox fruit;
        Label labelScore;
        // Объекты
        MediaPlayer backgroundsound = new MediaPlayer();
        SoundPlayer eatFruitSong = new SoundPlayer();
        Random rnd = new Random();
        // Поля, массив
        PictureBox[] snake = new PictureBox[400];
        string path = "records.txt";
        int rI, rJ;
        int dirX, dirY;        
        int sizeOfSides = 40;
        int BestScore;
        int score;
        byte endg;           
        public Form1()
        {
            InitializeComponent();
            // Размер окна
            Width = 900;
            Height = 800;
            // Запрет изменения размера
            FormBorderStyle = FormBorderStyle.FixedSingle;
            // Изменение фона, музыка, параметры 
            BackColor = System.Drawing.Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            backgroundsound.Open(new Uri("backgroundMusic.wav", UriKind.Relative));
            eatFruitSong.Stream = Properties.Resources.eatfruitmusic;
            BestScore = Properties.Settings.Default.bestrecords; 
            // Переменные 
            dirX = 1;
            dirY = 0;
            // Вывод очков
            labelScore = new Label();
            labelScore.Font = new Font(labelScore.Font.Name, 12, labelScore.Font.Style); 
            labelScore.Text = "Очки: 0";            
            labelScore.Location = new Point(810, 10);
            Controls.Add(labelScore);
            // Создание фрукта
            fruit = new PictureBox();
            fruit.Size = new Size(sizeOfSides, sizeOfSides);
            // Генерация карты, фрукта, змеи, обновление скорости
            generateMap();
            generateFruit();
            generateSnake();
            updateSpeed();
            // Таймер для контроля скорости игры
            timer1.Tick += new EventHandler(update);
            timer1.Start();
            // Счёт нажатия клавиш
            KeyDown += new KeyEventHandler(OKP);
            // Фоновая музыка
            backgroundsound.Play();
            backgroundsound.MediaEnded += Backgroundsound_MediaEnded;
        }
        // Действие при окончании фоновой музыки
        private void Backgroundsound_MediaEnded(object sender, EventArgs e)
        {
            backgroundsound.Open(new Uri("backgroundMusic.wav", UriKind.Relative));
            backgroundsound.Play();
        }
        // Появление фрукта в рандомном месте
        void generateFruit()
        {
            Random r = new Random();
            rI = r.Next(0, Height - sizeOfSides);
            int tempI = rI % sizeOfSides;
            rI -= tempI;
            rJ = r.Next(0, Height - sizeOfSides);
            int tempJ = rJ % sizeOfSides;
            rJ -= tempJ;
            rI++;
            rJ++;
            fruit.Location = new Point(rI, rJ);
            fruit.SizeMode = PictureBoxSizeMode.Zoom;
            fruit.Image = Properties.Resources.fruit;
            Controls.Add(fruit);
        }
        // Появление змеи в рандомном месте
        void generateSnake()
        {
            snake[0] = new PictureBox();
            snake[0].Location = new Point(201, 201);
            snake[0].Size = new Size(sizeOfSides - 1, sizeOfSides - 1);
            snake[0].SizeMode = PictureBoxSizeMode.Zoom;
            snake[0].Image = Properties.Resources.headOfSnake;
            Controls.Add(snake[0]);
        }
        // Проверка на столкновение змеи со стенками
        void checkBorders()
        {
            if (snake[0].Location.X < 0)
            {
                for (int i = 1; i <= score; i++)
                {
                    Controls.Remove(snake[i]);
                }
                updateSpeed();
                BestRecords();
                end();
                dirX = 1;
            }
            if (snake[0].Location.X > Height)
            {
                for (int i = 1; i <= score; i++)
                {
                    Controls.Remove(snake[i]);
                }
                updateSpeed();
                BestRecords();
                end();
                dirX = -1;
            }
            if (snake[0].Location.Y < 0)
            {
                for (int i = 1; i <= score; i++)
                {
                    Controls.Remove(snake[i]);
                }
                updateSpeed();
                BestRecords();
                end();
                dirY = 1;
            }
            if (snake[0].Location.Y > Height)
            {
                for (int i = 1; i <= score; i++)
                {
                    Controls.Remove(snake[i]);
                }
                updateSpeed();
                BestRecords();
                end();
                dirY = -1;
            }
        }
        // Конец игры
        void end()
        {
            if (endg == 0)
            {
                backgroundsound.Stop();
                timer1.Stop();
                endg = 1;
                string message = "Вы проиграли, ваш счёт: " + score + "\nВаш рекорд: " + BestScore;
                score = 0;
                string caption = "Продолжить?";
                if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {                                       
                    endg = 0;
                    snake[0].Location = new Point(401, 401);
                    updateSpeed();
                    backgroundsound.Play();
                    labelScore.Text = "Очки: " + score;
                    timer1.Start();
                }
                else
                {
                    Application.Exit();
                }
            }                  
        }
        // Сохранение лучшего рекорда
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
        // Действия при съеденнии фрукта
        void eatFrute()
        {
            if (snake[0].Location.X == rI && snake[0].Location.Y == rJ)
            {               
                labelScore.Text = "Очки: " + ++score;
                eatFruitSong.Play();
                snake[score] = new PictureBox();
                snake[score].Location = new Point(snake[score - 1].Location.X + 40 * dirX, snake[score - 1].Location.Y - 40 * dirY);
                snake[score].Size = new Size(sizeOfSides - 1, sizeOfSides - 1);
                snake[score].BackColor = System.Drawing.Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                BackColor = System.Drawing.Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                Controls.Add(snake[score]);
                updateSpeed();
                generateFruit();
            }
        }  
        // Действия при попадании змеи по самой себе
        void eatItself()
        {
            for (int i = 1; i < score; i++)
            {
                if (snake[0].Location == snake[i].Location)
                {
                    for (int j = i; j <= score; j++)
                    {
                        Controls.Remove(snake[j]);
                        Controls.Remove(snake[1]);
                    }
                    updateSpeed();
                    BestRecords();
                    end();
                }
            }
        }
        // Контроль скорости игры в зависимости от количества съеденных фруктов
        void updateSpeed()
        {
            int timeInterval = 250;
           if (0 <= score && score <  5) { timeInterval = 200; }
           else if (5 <= score && score < 10) { timeInterval = 150; }
           else if (10 <= score && score < 15) { timeInterval = 100; }
           else if (15 <= score && score <  25) { timeInterval = 50; }
           else { timeInterval = 20; }
            timer1.Interval = timeInterval;
        }
        // Разметка поля игры
        void generateMap()
        {
            for (int i = 0; i < Width / sizeOfSides; i++)
            {
                PictureBox pic = new PictureBox();
                pic.BackColor = System.Drawing.Color.Blue;
                pic.Location = new Point(0, sizeOfSides * i);
                pic.Size = new Size(Width - 100, 1);
                Controls.Add(pic);
            }
            for (int i = 0; i <= Height / sizeOfSides; i++)
            {
                PictureBox pic = new PictureBox();
                pic.BackColor = System.Drawing.Color.Blue;
                pic.Location = new Point(sizeOfSides * i, 0);
                pic.Size = new Size(1, Width);
                Controls.Add(pic);
            }
        }
        // Движение змеи
        void moveSnake()
        {
            for (int i = score; i >= 1; i--)
            {
                snake[i].Location = snake[i - 1].Location;
            }
            snake[0].Location = new Point(snake[0].Location.X + dirX * (sizeOfSides), snake[0].Location.Y + dirY * (sizeOfSides));
            eatItself();
        }
        // Обновлени при каждом движении змеи
        void update(object obj, EventArgs eventsArgs)
        {
            checkBorders();
            eatFrute();
            moveSnake();           
        }
        // Управление змеёй и пауза
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
                    MessageBox.Show("Пауза! Ваши очки: " + score + "\nВаш рекорд: " + BestScore + "\nЧтобы продолжить нажмите 'ОК'!");
                    timer1.Start();
                    break;
            }
        }       
    }
}
