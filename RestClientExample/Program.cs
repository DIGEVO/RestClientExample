using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestClientExample
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessFinancialData().Wait();
            Console.ReadKey();
        }

        enum Indicators {
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

        private static async Task ProcessFinancialData()
        {
            var client = new HttpClient();
            var urlString = "http://www.mindicador.cl/api/{0}/{1}";

            //var stringTask = client.GetStringAsync(String.Format(urlString, Indicators.dolar, DateTime.Now.ToString("dd-MM-yyyy")));

            var enumArray = Enum.GetValues(typeof(Indicators));
            enumArray.GetValue(0);
            var r = new Random();
            
            var stringTask = client.GetStringAsync($"http://www.mindicador.cl/api/{enumArray.GetValue(r.Next(enumArray.Length))}/{DateTime.Now.ToString("dd-MM-yyyy")}");

            var msg = await stringTask;
            Console.Write(msg);
        }
    }
}
