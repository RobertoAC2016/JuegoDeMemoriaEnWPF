using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFMemorama
{
    public partial class ImagesWindow : Window
    {
        private string path = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}imgs{Path.DirectorySeparatorChar}";
        private string db = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}db.txt";
        public string basura = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}imgs{Path.DirectorySeparatorChar}trash.png";
        public string apath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}";
        private List<Objeto_Imagen> imagenes = new List<Objeto_Imagen>();
        public ImagesWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cargar_datos();
        }
        private void cargar_datos()
        {
            imagenes = obtener_imagenes();
            lstCarrucel.ItemsSource = imagenes;
        }
        private List<Objeto_Imagen> obtener_imagenes()
        {
            imagenes = new List<Objeto_Imagen>();
            var txt = File.ReadAllText(db);
            if (txt != null) imagenes = JsonConvert.DeserializeObject<List<Objeto_Imagen>>(txt);
            foreach (var i in imagenes)
            {
                i.impath = $"{path}{i.name}";
                i.apath = $"{apath}trash.png";
            }
            return imagenes;
        }
        private void addFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = "C:\\";
            open.Filter = images_per_level.GetImagesFilter();//aqui necesitamos solo generar la funcion q nos devuelva los formatos permitidos de imagen
            open.FilterIndex = 2;
            open.RestoreDirectory = true;
            if (open.ShowDialog() == DialogResult.GetValueOrDefault(true))
            {
                var filepath = open.FileName;
                var secureFileName = open.SafeFileName;
                byte[] b = File.ReadAllBytes(filepath);

                var file_name = $"{imagenes.Count}.png";

                var pfile = $"{path}{file_name}";
                using var stream = File.Create(pfile);
                stream.Write(b, 0, b.Length);
                AgregarNuevaImagen(new Objeto_Imagen {
                    indice = imagenes.Count,
                    name = file_name,
                    impath = pfile,
                    apath = $"{apath}trash.png"
                });
            }
        }
        //aqui finalmente agregamos la imagen a la lista de imagenes
        private void AgregarNuevaImagen(Objeto_Imagen obj)
        {
            imagenes.Add(obj);
            File.WriteAllText(db, JsonConvert.SerializeObject(imagenes));
            cargar_datos();
        }
        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Image;
            imagenes = imagenes.Where(x => x.indice != int.Parse(btn.Tag.ToString())).ToList();
            File.WriteAllText(db, JsonConvert.SerializeObject(imagenes));
            cargar_datos();
        }
    }
}
