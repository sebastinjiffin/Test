using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldMax.AsyncService.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started");
            ConfigureService.Configure();
            Console.WriteLine("Completed");

        }
    }
}
