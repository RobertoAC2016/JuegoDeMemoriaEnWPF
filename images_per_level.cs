using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace WPFMemorama
{
    public class images_per_level
    {
        //Empezamos por las variables globales
        #region VARIABLES
        //esta variable contiene la ruta de nuestro archivo de Base de datos
        private string db = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}db.txt";
        private Random rnd = new Random();//este random nos ayudara a saber q imagenes y en q posicion deben aparecer
        public int level { get; set; } = 1;//nivel inicial
        public String img0 { get; set; } = "blank.png";//las imagenes empiezan vacias
        public String img1 { get; set; } = "blank.png";
        public String img2 { get; set; } = "blank.png";
        public String img3 { get; set; } = "blank.png";
        private List<int> tren = new List<int>();//esta variable contendra solo las imagenes del nivel seleccionado 1 a 4
        private List<int> imgs = new List<int>();//esta variable contendra todas las imagenes de nuestra Base de datos
        private List<int> imgs_to_show = new List<int>();
        public String image_to_find { get; set; } = "blank.png";
        private List<Objeto_Imagen> imagenes = new List<Objeto_Imagen>();
        #endregion
        public images_per_level()
        {
            img0 = "blank.png";
            img1 = "blank.png";
            img2 = "blank.png";
            img3 = "blank.png";
            image_to_find = "blank.png";
        }
        private List<Objeto_Imagen> obtener_imagenes()
        {
            imagenes = new List<Objeto_Imagen>();
            var txt = File.ReadAllText(db);
            if (txt != null) imagenes = JsonConvert.DeserializeObject<List<Objeto_Imagen>>(txt);
            return imagenes;
        }
        public void Start(int useLevel)
        {
            tren = new List<int>();
            imgs = new List<int>();
            var cnt = obtener_imagenes();
            foreach (var i in cnt)
            {
                imgs.Add(i.indice);
            }
            var r = rnd.Next(0, 4);
            if (useLevel == 1)
            {
                tren.Add(r);
            }
            else
            {
                while (true)
                {
                    var len = tren.Count;
                    if (len < useLevel)
                    {
                        r = rnd.Next(0, 4);
                        if (tren.Where(x => x.Equals(r)).Count() <= 0) tren.Add(r);
                    }
                    else break;
                }
            }
            imgs_to_show = select_image(tren.Count);
            int idx = 0;
            foreach (int img in tren)
            {
                if (img == 0) img0 = $"{imgs_to_show[idx]}.png";
                if (img == 1) img1 = $"{imgs_to_show[idx]}.png";
                if (img == 2) img2 = $"{imgs_to_show[idx]}.png";
                if (img == 3) img3 = $"{imgs_to_show[idx]}.png";
                idx++;
            }
            r = rnd.Next(0, tren.Count);
            image_to_find = $"{imgs_to_show[r]}.png";
        }
        private List<int> select_image(int count)
        {
            var r = rnd.Next(0, imgs.Count);
            List<int> ims = new List<int>();
            if (count == 1)
            {
                ims.Add(r);
            }
            else
            {
                while (true)
                {
                    if (ims.Count < count)
                    {
                        r = rnd.Next(0, imgs.Count);
                        if (ims.Where(x => x.Equals(r)).Count() <= 0) ims.Add(r);
                    }
                    else break;
                }
            }
            return ims;
        }
        public static string GetImagesFilter()
        {
            String imageExtensions = String.Empty;
            String separador = "";
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            Dictionary<String, String> imageFilters = new Dictionary<string, string>();
            foreach (ImageCodecInfo codec in codecs)
            {
                imageExtensions = $"{imageExtensions}{separador}{codec.FilenameExtension.ToLower()}";
                separador = ";";
                imageFilters.Add($"{codec.FormatDescription} files ({codec.FilenameExtension.ToLower()})", codec.FilenameExtension.ToLower());
            }
            String result = String.Empty;
            separador = "";
            foreach (KeyValuePair<String, String> filter in imageFilters)
            {
                result += $"{separador}{filter.Key}|{filter.Value}";
                separador = "|";
            }
            if (!String.IsNullOrEmpty(imageExtensions))
            {
                result += $"{separador}Image files|{imageExtensions}";
            }
            return result;
        }
    }
    public class Objeto_Imagen
    {
        public int indice { get; set; } = 0;
        public string name { get; set; } = "";
        public string impath { get; set; } = "";
        public string apath { get; set; } = "";//path alternativo
    }
}