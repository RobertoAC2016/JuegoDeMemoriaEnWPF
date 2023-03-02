using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace WPFMemorama
{
    public partial class MainWindow : Window
    {
        //emperare con las variables globales
        #region VARIABLES
        //esta clase la desarrollaremos primero para q nos ayude con la logica del manejo de imagenes
        public images_per_level juego = new images_per_level();
        //Esta variable path solo nos servira para indicar en q ruta se almacenan las imagenes
        private String path = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}imgs{Path.DirectorySeparatorChar}";
        private int level = 1;        //esta variable nos indicara en q nivel esta jugando el usuario
        private bool activo = false;  //esta variable nos indicara si esta jugando o no
        private Timer t = new Timer();//este timer nos ayudara a reducir cada segundo el contador para q el jugandor vea q su tiempo se esta terminando
        private const int tiempo = 5; //Esta es la variable global q indica cuando segundos tiene el usuario para resulver el puzzle
        private int contador = 0;     //Esta variable es la q se va a ir disminuyendo y se mostrar al usuario, relacionada al tiempo
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            t.Interval = 1000;
            t.Enabled = false;
            t.Elapsed += new ElapsedEventHandler(running);
            contador = tiempo;
            lbllevel.Content = "Nivel 1 de 4";
            lbltitle.Content = $"Pon atencion a las imagenes y marca donde se encontraba";
            lblnumber.Content = "";
        }
        private void running(object? sender, ElapsedEventArgs e)
        {
            //Dado q este metodo es llamada desde un evento alterno a la aplicacion principal el elemento q
            //usaremos para asignar el numero de segundos q quedan para resolver el puzzle debemos usar un delegado
            Dispatcher.Invoke(() => {
                lblnumber.Content = $"{contador}";
            });
            if (contador <= 0)
            {
                contador= tiempo;
                t.Stop();
                ocultar_imagenes();
            }
            contador--;
            actualizar_nivel();
        }
        private void actualizar_nivel()
        {
            Dispatcher.Invoke(() => {
                lbllevel.Content = $"Nivel {level} de 4";
            });
        }
        private void ocultar_imagenes()
        {
            Dispatcher.Invoke(() => {
                lblnumber.Content = "";
                imgfind.Source = new BitmapImage(new Uri($"{path}{juego.image_to_find}"));
                imgfind.Tag = juego.image_to_find;
                img0.Source = new BitmapImage(new Uri($"{path}blank.png"));
                img1.Source = new BitmapImage(new Uri($"{path}blank.png"));
                img2.Source = new BitmapImage(new Uri($"{path}blank.png"));
                img3.Source = new BitmapImage(new Uri($"{path}blank.png"));
            });
            activo = true;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            iniciar_juego();
        }
        private void iniciar_juego()
        {
            juego = new images_per_level();
            img0.Source = new BitmapImage(new Uri($"{path}{juego.img0}"));
            img1.Source = new BitmapImage(new Uri($"{path}{juego.img1}"));
            img2.Source = new BitmapImage(new Uri($"{path}{juego.img2}"));
            img3.Source = new BitmapImage(new Uri($"{path}{juego.img3}"));
            imgfind.Source = new BitmapImage(new Uri($"{path}{juego.image_to_find}"));
            lbllevel.Content = $"Nivel {level} de 4";
        }
        private void btnimgadmin_Click(object sender, RoutedEventArgs e)
        {
            ImagesWindow win = new ImagesWindow();
            win.ShowDialog();
        }
        private void compara_imagenes(object sender, MouseButtonEventArgs e)
        {
            if (activo)
            {
                var img = sender as Image;
                if (img.Tag.Equals(imgfind.Tag))
                {
                    imgresult.Source = new BitmapImage(new Uri($"{path}aok.png"));
                    activo = false;
                }
                else
                {
                    imgresult.Source = new BitmapImage(new Uri($"{path}aerror.png"));
                    activo = false;
                }
            }
        }
        private void btnstart_Click(object sender, RoutedEventArgs e)
        {
            juego = new images_per_level();
            juego.Start(level);
            img0.Source = new BitmapImage(new Uri($"{path}{juego.img0}"));img0.Tag = juego.img0;
            img1.Source = new BitmapImage(new Uri($"{path}{juego.img1}"));img1.Tag = juego.img1;
            img2.Source = new BitmapImage(new Uri($"{path}{juego.img2}"));img2.Tag = juego.img2;
            img3.Source = new BitmapImage(new Uri($"{path}{juego.img3}"));img3.Tag = juego.img3;
            imgfind.Source = new BitmapImage(new Uri($"{path}blank.png"));
            imgfind.Tag = "";
            imgresult.Source = new BitmapImage(new Uri($"{path}blank.png"));
            contador = tiempo;
            t.Start();
            lbllevel.Content = $"Nivel {level} de 4";
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            level++;
            if (level >= 4) level = 4;
            lbllevel.Content = $"Nivel {level} de 4";
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            level--;
            if (level <= 1) level = 1;
            lbllevel.Content = $"Nivel {level} de 4";
        }
    }
}
