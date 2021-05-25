using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pylot
{

    public struct data
    {
        public double x;    //x наших метки и визира. Время.
        public double yt;   //y метки
        public double yp;   //y визира
        public double deltapath;
    }
    public static class Results
    {
        public static int Number;    //Номер экперимента. Подхватывается из app.config
        public static DateTime Date;    //Дата и время. Устанавливаются в момент начала эксперимента
        public static string Name;      //ФИО испытуемого
        public static string Age;
        public static double targetspeed = 0.3;
        public static double sensivity = 40;
        public static double Height = 0;
        public static double halfHeight = 0;
        public static double timing = 0.1;
        public static double stratborder = 70;
        public static int TimeToDissapeare = 40;
        public static int optRamTime = 40;
        public static List<data> datas = new List<data>();
    }
}
