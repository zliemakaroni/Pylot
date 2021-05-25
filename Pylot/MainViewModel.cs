using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace Pylot
{
    class MainViewModel
    {

        public IList<DataPoint> FormingTargetPoints { get; private set; }       //Списки для графиков в результатах
        public IList<DataPoint> FormingPointerPoints { get; private set; }
        public IList<DataPoint> BlindTargetPoints { get; private set; }
        public IList<DataPoint> BlindPointerPoints { get; private set; }
        public IList<DataPoint> DiffPoints { get; private set; }



        public MainViewModel()
        {
            this.FormingTargetPoints = new List<DataPoint>();
            this.FormingPointerPoints = new List<DataPoint>();
            this.BlindTargetPoints = new List<DataPoint>();
            this.BlindPointerPoints = new List<DataPoint>();
            this.DiffPoints = new List<DataPoint>();
            foreach (var data in Results.datas)         //Вбиваем в списки значения
            {
                if (data.x <= Results.TimeToDissapeare)
                {
                    FormingTargetPoints.Add(new DataPoint(data.x, -Math.Round((data.yt - Results.halfHeight) * 0.26458333, 2)));
                    FormingPointerPoints.Add(new DataPoint(data.x, -Math.Round((data.yp - Results.halfHeight) * 0.26458333, 2)));
                }
                else
                {
                    BlindTargetPoints.Add(new DataPoint(data.x, -Math.Round((data.yt - Results.halfHeight) * 0.26458333, 2)));
                    BlindPointerPoints.Add(new DataPoint(data.x, -Math.Round((data.yp - Results.halfHeight) * 0.26458333, 2)));
                }
            }

            for(int i = 1; i < Results.datas.Count(); i++)
            {
                DiffPoints.Add(new DataPoint(Results.datas[i].x, Math.Abs(Results.datas[i].deltapath / (Results.Height*2))));
            }

        }

    }
}
