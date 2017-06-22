using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestClientExample
{
    class DataPoint
    {
        public DataPoint(DateTime Date, float Value) { this.Date = Date; this.Value = Value; }
        public DateTime Date { get; set; }
        public float Value { get; set; }

        public override string ToString() { return $"{Date} {Value}"; }
    }

    class Program
    {

        static void Main(string[] args)
        {
            //if (startDate > endDate) throw new ArgumentException("Start time need to be before end time!");

            //var task = GetFinancialData(Indicators.dolar, DateTime.Parse("01-02-2017"), DateTime.Now);
            //task.Wait();
            //task.Result.ToList().ForEach(Console.WriteLine);

            var task = GetFinancialData(Indicators.dolar, DateTime.Now);
            task.Wait();
            Console.WriteLine(task.Result);

            Console.ReadKey();
        }

        enum Indicators
        {
            uf                      //Unidad de fomento.
            , ivp                   //Indice de valor promedio.
            , dolar                 //Dólar observado.
            , dolar_intercambio     //Dólar acuerdo.
            , euro                  //euro.
            , ipc                   //Índice de Precios al Consumidor
            , utm                   //Unidad Tributaria Mensual.
            , imacec                //Imacec.
            , tpm                   //Tasa Política Monetaria.
            , libra_cobre           //Libra de Cobre.
            , tasa_desempleo        //Tasa de desempleo.
        }

        private static async Task<DataPoint> GetFinancialData(Indicators indicator, DateTime date)
        {
            var client = new HttpClient();
            var url = $@"http://www.mindicador.cl/api/{indicator}/{date.ToString("dd-MM-yyyy")}";
            var stringTask = client.GetStringAsync(url);
            var stringData = await stringTask;
            var token = JToken.Parse(stringData);
            var jarraydata = token["serie"] ?? new JArray();

            var result = new DataPoint(DateTime.MinValue, 0.0f);

            if (jarraydata.HasValues)
            {
                var data = jarraydata.ToArray()[0];
                result.Date = data["fecha"].Value<DateTime>();
                result.Value = data["valor"].Value<float>();
            }

            return result;
        }

        private static async Task<IEnumerable<DataPoint>> GetFinancialData(Indicators indicator, DateTime startDate, DateTime endDate)
        {
            var client = new HttpClient();

            var url = String.Format("http://www.mindicador.cl/api/{0}",
                        startDate.Year == endDate.Year ?
                            $@"{indicator}/{startDate.ToString("yyyy")}" :
                            $"{indicator}");

            var stringTask = client.GetStringAsync(url);
            var stringData = await stringTask;
            var token = JToken.Parse(stringData);
            var jarraydata = token["serie"] ?? new JArray();

            return jarraydata
                .AsParallel()
                .Select(ob => new DataPoint(ob["fecha"].Value<DateTime>(), ob["valor"].Value<float>()))
                .Where(ob => ob.Date >= startDate && ob.Date <= endDate);
        }
    }
}
