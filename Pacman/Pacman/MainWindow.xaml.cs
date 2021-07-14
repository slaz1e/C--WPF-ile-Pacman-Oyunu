using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;// threading özelliğini kullanabilmek sisteme bunu ekliyoruz

namespace Pacman
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();//DispatcherTimer türünde gameTimer üretiyoruz
        // hayaletlerin ve pacmanin hareketleri, hitbox, hız gibi tanımlamalar yapılıyor
        bool goLeft, goRight, goDown, goUp;
        bool noLeft, noRight, noDown, noUp;
        int speed = 8;
        Rect pacmanHitbox;
        int ghostSpeed = 10;
        int ghostMoveStep = 100;
        int currentGhostStep;
        int score = 0;

        public MainWindow()
        {
            InitializeComponent();
            GameSetUp();//GameSetUp fonksiyonunu çağırıyoruz.
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)//bu satır tuşa basma eventi
        {
            if (e.Key == Key.Left && noLeft == false)
            {
                goRight = goUp = goDown = false;//sol hariç tüm yönleri false yapıyoruz
                noRight = noUp = noDown = false;//sol hariç tüm kısıtlamaları false yapıyoruz

                goLeft = true;//sola gitmeyi true değerine çeviriyoruz

                pacman.RenderTransform = new RotateTransform(-180, pacman.Width /2 , pacman.Height /2 );//pacmani basılan yöne çeviriyoruz
            }
            //52-76 satır arasındaki kodlar sola gitme eventini diğer tüm yönler için aynı şekilde gerçekleştiriyor
            if (e.Key==Key.Right && noRight == false)
            {
                noLeft = noUp = noDown = false;
                goLeft = goUp = goDown = false;
                goRight = true;
                pacman.RenderTransform = new RotateTransform(0, pacman.Width / 2, pacman.Height / 2);

            }
            if (e.Key== Key.Up && noUp == false)
            {
                noRight = noDown = noLeft = false;
                goRight = goDown = goLeft = false;
                goUp = true;
                pacman.RenderTransform = new RotateTransform(-90, pacman.Width / 2, pacman.Height / 2);

            }
            if (e.Key== Key.Down && noDown == false)
            {
                noUp = noLeft = noRight = false;
                goUp = goLeft = goRight = false;
                goDown = true;
                pacman.RenderTransform = new RotateTransform(90, pacman.Width / 2, pacman.Height / 2);
            }
            //
        }
        private void GameSetUp()//program çalıştığı anda bu fonksiyonda çalışıyor
        {
            MyCanvas.Focus();//MyCanvas'ı program için ana odak haline getiriyor
            gameTimer.Tick += GameLoop;//GameLoop eventini Zaman tikiyle bağdaştırıyor
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);//Zaman tikinin her 20 milisaniyede gerçekleştirilmesini sağlıyor
            gameTimer.Start();//zamanı başlatıyor
            currentGhostStep = ghostMoveStep;
            //Pacman ve hayaletlerin görsellerini import ediyor
            ImageBrush pacmanImage = new ImageBrush();
            pacmanImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resimler/pacman.png"));
            pacman.Fill = pacmanImage;
            
            ImageBrush Ghost = new ImageBrush();
            Ghost.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resimler/ghost.png"));
            redGuy.Fill = Ghost;

            ImageBrush Ghost1 = new ImageBrush();
            Ghost.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resimler/ghost.png"));
            redGuy1.Fill = Ghost;

            ImageBrush Ghost2 = new ImageBrush();
            Ghost.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resimler/ghost.png"));
            redGuy2.Fill = Ghost;
            //
        }

        private void GameLoop(object sender, EventArgs e)//Bu eventin içerisinde tüm yön kontrolleri hitbox çarpışmaları
         //ayarlanıyor
        {
            txtScore.Content = "Score: " + score;
            if (goRight)//goRight boolean'i eğer true değerindeyse pacman sağa gidiyor
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);
            }
            if (goLeft)//goLeft boolean'i eğer true değerindeyse pacman sola gidiyor
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - speed);

            }
            if (goUp)//goUp boolean'i eğer true değerindeyse pacman yukarı gidiyor
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) - speed);
            }
            if (goDown)//goDown boolean'i eğer true değerindeyse pacman aşağı gidiyor
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) + speed);
            }
            if (goDown && Canvas.GetTop(pacman) + 80 > Application.Current.MainWindow.Height)//Burada ise pacman'in pencere dışarısına çıkmasını önleyen kodlar yer alıyor
            {
                //pacman aşağı giderken pencerenin içine çarpıyorsa hareketi durduruyor
                noDown = true;
                goDown = false;
            }
            if(goUp && Canvas.GetTop(pacman) < 1)
            {
                noUp = true;
                goUp = false;
            }
            if(goLeft && Canvas.GetLeft(pacman) -10 < 1)
            {
                noLeft = true;
                goLeft = false;

            }
            if(goRight && Canvas.GetLeft(pacman) + 70 > Application.Current.MainWindow.Width)
            {
                noRight = true;
                goRight = false;
            }

            pacmanHitbox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman),pacman.Width,pacman.Height);//Pacmanin hitboxı tanımlanıyor

            foreach(var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitbox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if ((string)x.Tag == "wall")//Wall isminde bileşenlerle temasa geçerse pacmanin hareketini kısıtlıyor yani duvarlardan geçmeyi önlüyor.
                {
                    if(goLeft==true && pacmanHitbox.IntersectsWith(hitbox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + 10);
                        noLeft = true;
                        goLeft = false;
                    }
                    if (goRight == true && pacmanHitbox.IntersectsWith(hitbox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 10);
                        noRight = true;
                        goRight = false;
                    }
                    if (goDown == true && pacmanHitbox.IntersectsWith(hitbox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) - 10);
                        noDown = true;
                        goDown = false;
                    }
                    if (goUp == true && pacmanHitbox.IntersectsWith(hitbox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) + 10);
                        noUp = true;
                        goUp = false;
                    }
                }
                if ((string)x.Tag == "coin")//pacman coinler ile temas ederse coinleri görünmez yapıp skorumuzu 1 arttırıyor
                {
                    if (pacmanHitbox.IntersectsWith(hitbox) && x.Visibility==Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;

                    }
                    
                }
                if ((string)x.Tag == "ghost")//hayaletlerle temas edilirse oyunu sonlandırıp ekrana messagebox yolluyoruz
                {
                    if (pacmanHitbox.IntersectsWith(hitbox))//eğer pacman ghost tagında bir hitbox ile çarpışırsa
                    {
                        GameOver("Hayalete yakalandın, tekrar oynamak için tamama basın");
                    }
                    if (x.Name.ToString() == "redGuy")//redGuy isminde bir rectangle varsa eğer
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ghostSpeed);//rectangle'ı sola doğru hareket ettir
                    }
                    else
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }
                    currentGhostStep--;
                    //hayalet bir kez git gel yaptığında başa sarıp hareketi tekrar yaptırıyoruz
                    if (currentGhostStep < 1)
                    {
                        currentGhostStep = ghostMoveStep;
                        ghostSpeed = -ghostSpeed;
                    }
                }
                
            }
            //Oyuncu 90 coini toplarsa ekrana geçerli mesajı yazdırıyoruz
            if (score == 90)
            {
                GameOver("Tüm altınları topladınız ve KAZANDINIZ!");
            }
        }
        

        private void GameOver(string message)//Oyun biterse bu fonksiyon kullanılıyor
        {
            gameTimer.Stop();//gameTimer'ı durduruyoruz.
            MessageBox.Show(message, "OYUN BİTTİ");//MessageBoxta
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
