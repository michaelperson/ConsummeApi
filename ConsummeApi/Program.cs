using ConsummeApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.DependencyInjection;
using RequesterTool;
using RequesterTool.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsummeApi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();
            try
            {
                IRequester req= provider.GetRequiredService<IRequester>();
                foreach(Employee item in  await req.Get<Employee>("Employees" ))
                {
                    DisplayInfo(item);
                }                     
            

                Employee emp = new Employee() { FirstName = "Patrick", LastName = "L'étoile" };
                if(await req.Post<Employee>("Employees", emp))
                {
                    Console.WriteLine("Employee inserted");
                }

                Employee empPut = new Employee() { FirstName = "Patrick", LastName = "Fiori" };
                if (await req.Put<Employee>(@"Employees\6", empPut))
                {
                    Console.WriteLine("Employee Changed");
                }
                 
                if (await req.Delete<int>(@"Employees\", 6))
                {
                    Console.WriteLine("Employee 6 Deleted");
                }

                JsonPatchDocument<Employee> jsonPatch = new JsonPatchDocument<Employee>();
                jsonPatch.Replace(e => e.FirstName, "Albert");

                if(await req.Patch<Employee, int>("Employees",5,jsonPatch))
                {
                    Console.WriteLine("Employee 5 modified");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex}");
            }

        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRequester, Requester>(m=> new Requester("https://localhost:44323/api/"));
        }

        private static void DisplayInfo(Employee emp)
        {
            Console.WriteLine($"Id: {emp.Id}");
            Console.WriteLine($"FirstName: {emp.FirstName}");
            Console.WriteLine($"LastName: {emp.LastName}"); 
        }
    }
}
