using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Pylot
{
    class ResultsWorker
    {
        private double lessBefore40Counter = 0;     //счетчик убывающих до 40 сек стратегий
        private double greaterBefore40Counter = 0;  //счетчик возрастающих до 40 сек стратегий
        private double lessAfter40Counter = 0;      //счетчик убывающих после 40 сек стратегий
        private double greaterAfter40Counter = 0;   //счетчик возрастающих после 40 сек стратегий
        private double less3540Counter = 0;         //счетчик убывающих в 35-40 сек стратегий
        private double greater3540Counter = 0;      //счетчик возрастающих в 35-40 сек стратегий
        private double vPhaseTimeB40 = 0;           //счетчик времени фаз восстановления до 40
        private double sPhaseTimeB40 = 0;           //счетчик времени фаз стабилизации до 40
        private double rPhaseTimeB40 = 0;           //счетчик времени фаз распада до 40
        private double vPhaseTimeA40 = 0;           //счетчик времени фаз восстановления после 40
        private double sPhaseTimeA40 = 0;           //счетчик времени фаз стабилизации после 40
        private double rPhaseTimeA40 = 0;           //счетчик времени фаз распада после 40
        private double Ram = 0;                     //Оперативная динамическая память
        private double KPkoof = 1;
        private double PPOkoof = 2;
        private double RAMkoof = 6;
        private double Phasekoof = 2;

        public double VPhaseTime
        {
            set {}
            get { return Math.Round(this.vPhaseTimeB40 + this.vPhaseTimeA40, 1); }
        }
        public double SPhaseTime
        {
            set {}
            get { return Math.Round(this.sPhaseTimeB40 + this.sPhaseTimeA40, 1); }
        }
        public double RPhaseTime
        {
            set {}
            get { return Math.Round(this.rPhaseTimeB40 + this.rPhaseTimeA40, 1); }
        }
        public double VPhaseTimeB40
        {
            set { }
            get { return Math.Round(this.vPhaseTimeB40, 1); }
        }
        public double SPhaseTimeB40
        {
            set { }
            get { return Math.Round(this.sPhaseTimeB40, 1); }
        }
        public double RPhaseTimeB40
        {
            set { }
            get { return Math.Round(this.rPhaseTimeB40, 1); }
        }
        public double VPhaseTimeA40
        {
            set { }
            get { return Math.Round(this.vPhaseTimeA40, 1); }
        }
        public double SPhaseTimeA40
        {
            set { }
            get { return Math.Round(this.sPhaseTimeA40, 1); }
        }
        public double RPhaseTimeA40
        {
            set { }
            get { return Math.Round(this.rPhaseTimeA40, 1); }
        }

        public double RAM
        {
            set { }
            get { return this.Ram; }
        }

        public double Before40LessStrategyPercent = 0;
        public double Before40GreaterStrategyPercent = 0;
        public double After40LessStrategyPercent = 0;
        public double After40GreaterStrategyPercent = 0;


        public ResultsWorker()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            double valid;

            if (double.TryParse(config.AppSettings.Settings["KPkoof"].Value, out valid))
                this.KPkoof = valid;

            if (double.TryParse(config.AppSettings.Settings["PPOkoof"].Value, out valid))
                this.PPOkoof = valid;

            if (double.TryParse(config.AppSettings.Settings["RAMkoof"].Value, out valid))
                this.RAMkoof = valid;

            if (double.TryParse(config.AppSettings.Settings["Phasekoof"].Value, out valid))
                this.Phasekoof = valid;

            for (int i = Results.datas.Count()-1; i > 0; i--)
            {
                Results.datas[i] = new data {x = Results.datas[i].x, yt = Results.datas[i].yt, yp = Results.datas[i].yp, deltapath = Results.datas[i-1].deltapath };
            }
                foreach (var data in Results.datas)             //Считаем стратегии
            {
                if (data.x < 0.875 * Results.TimeToDissapeare)
                {
                    if (data.deltapath > 0)
                        lessBefore40Counter++;
                    else
                        greaterBefore40Counter++;
                }
                else if (data.x > Results.TimeToDissapeare)
                {
                    if (data.deltapath > 0)
                        lessAfter40Counter++;
                    else
                        greaterAfter40Counter++;
                }
                else
                {
                    if (data.deltapath > 0)
                    {
                        lessBefore40Counter++;
                        less3540Counter++;
                    }
                    else
                    {
                        greaterBefore40Counter++;
                        greater3540Counter++;
                    }
                }
            }

            for (int i = 1; i < Results.datas.Count(); i++) // Проход по результатам с 2 до последнего. Считаем фазы
            {
                if (Results.datas[i].x < Results.TimeToDissapeare)  //До 40
                {
                    if (((Math.Abs(Results.datas[i].deltapath) - Math.Abs(Results.datas[i - 1].deltapath)) / Results.Height) < -0.03) //Проверяем на уменьшение и приводим к условным единицам
                        vPhaseTimeB40 += Results.timing;
                    else if (((Math.Abs(Results.datas[i].deltapath) - Math.Abs(Results.datas[i - 1].deltapath)) / Results.Height) > 0.03) //Проверяем на увеличение и приводим к условным единицам
                        rPhaseTimeB40 += Results.timing;
                    else
                        sPhaseTimeB40 += Results.timing;
                }
                else                                                //После 40
                { 
                 if (((Math.Abs(Results.datas[i].deltapath) - Math.Abs(Results.datas[i - 1].deltapath)) / Results.Height) < -0.03) //Проверяем на уменьшение и приводим к условным единицам
                        vPhaseTimeA40 += Results.timing;
                    else if (((Math.Abs(Results.datas[i].deltapath) - Math.Abs(Results.datas[i - 1].deltapath)) / Results.Height) > 0.03) //Проверяем на увеличение и приводим к условным единицам
                        rPhaseTimeA40 += Results.timing;
                    else
                        sPhaseTimeA40 += Results.timing;
                }
            }

            Ram = Math.Round((Results.datas.Last().x - Results.optRamTime) / Results.optRamTime, 2);    //рассчет оперативной динамической памяти

            Before40LessStrategyPercent = Math.Round(lessBefore40Counter / (lessBefore40Counter + greaterBefore40Counter) * 100);        //% убывающих до 40 сек стратегий
            Before40GreaterStrategyPercent = Math.Round(greaterBefore40Counter / (lessBefore40Counter + greaterBefore40Counter) * 100);   //% возростающих до 40 сек стратегий
            After40LessStrategyPercent = Math.Round(lessAfter40Counter / (lessAfter40Counter + greaterAfter40Counter) * 100);              //% убывающих после 40 сек стратегий
            After40GreaterStrategyPercent = Math.Round(greaterAfter40Counter / (lessAfter40Counter + greaterAfter40Counter) * 100);        //% возростающиз после 40 сек стратегий
        }

        public string RamRes()
        {
            if (Ram >= 0.75)
                return Ram + "      Коп ≥ 0.75";
            else if (Ram >= 0.4 && Ram < 0.75)
                return Ram + "      0.4 ≤ Коп < 0.75";
            else if (Ram >= 0.2 && Ram < 0.4)
                return Ram + "      0.2 ≤ Коп < 0.4";
            else
                return Ram + "      Коп < 0.2";
        }

        private string StratRes(double less, double greater)
        {
            if (Math.Round((greater / (less + greater)) * 100) > Results.stratborder)
                return "Упреждающая";
            else if (Math.Round((less / (less + greater)) * 100) > Results.stratborder)
                return "Запаздывающая";
            else if (less > greater)
                return "Запаздывающе-упреждающая";
            else
                return "Упреждающе-запаздывающая";
        }

        public string StratAfter40Res()
        {
            return StratRes(lessAfter40Counter, greaterAfter40Counter);
        }

        public string StratBefore40Res()
        {
            return StratRes(lessBefore40Counter, greaterBefore40Counter);
        }

        public string Strat3540Res()
        {
            return StratRes(less3540Counter, greater3540Counter);
        }
        public string FinPhaseRes()
        {
            if (Math.Round(vPhaseTimeA40, 1) + Math.Round(sPhaseTimeA40, 1) == 0)
                return "В + С = 0";
            else if (Math.Round(vPhaseTimeA40, 1) + Math.Round(sPhaseTimeA40, 1) < Math.Round(rPhaseTimeA40, 1))
                return "В + С < Р";
            else if (Math.Round(vPhaseTimeA40, 1) + Math.Round(sPhaseTimeA40, 1) > Math.Round(rPhaseTimeA40, 1))
                return "В + С > Р";
            else if (Math.Round(vPhaseTimeA40, 1) > Math.Round(rPhaseTimeA40, 1) && Math.Round(vPhaseTimeA40, 1) > Math.Round(sPhaseTimeA40, 1))
                return "Р < В > С";
            else
                return "Непонятно";
        }
        public string FinalMark()
        {
            double mark=0;
            switch(this.Strat3540Res())
            {
                case "Упреждающая":
                    mark += 3*this.KPkoof;
                    break;
                case "Упреждающе-запаздывающая":
                    mark += 2 * this.KPkoof;
                    break;
                case "Запаздывающе-упреждающая":
                    mark += 1 * this.KPkoof;
                    break;
                default:
                    break;
            }
            switch (this.StratAfter40Res())
            {
                case "Упреждающая":
                    mark += 3 * this.PPOkoof;
                    break;
                case "Упреждающе-запаздывающая":
                    mark += 2 * this.PPOkoof;
                    break;
                case "Запаздывающе-упреждающая":
                    mark += 1 * this.PPOkoof;
                    break;
                default:
                    break;
            }
            switch (this.FinPhaseRes())
            {
                case "Р < В > С":
                    mark += 3 * this.Phasekoof;
                    break;
                case "В + С > Р":
                    mark += 2 * this.Phasekoof;
                    break;
                case "В + С < Р":
                    mark += 1 * this.Phasekoof;
                    break;
                default:
                    break;
            }
            if (Ram >= 0.75)
                mark += 3 * this.RAMkoof;
            else if (Ram >= 0.4 && Ram < 0.75)
                mark += 2 * this.RAMkoof;
            else if (Ram >= 0.2 && Ram < 0.4)
                mark += 1 * this.RAMkoof;

            double maxMark = ((this.KPkoof + this.PPOkoof + this.RAMkoof + this.Phasekoof) * 3);

            if (mark / maxMark > 0.75)
                return "Высокий (отл)";
            else if (mark / maxMark > 0.5 && mark / maxMark <= 0.75)
                return "Средний (хор)";
            else if (mark / maxMark > 0.25 && mark / maxMark <= 0.5)
                return "Ниже среднего (уд)";
            else
                return "Низкий (неуд)";
        }
    }
}
