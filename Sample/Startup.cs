using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EC2ResourceDetection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
namespace Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private readonly IHttpClientFactory _clientFactory;

        public Startup(IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {


            // Initialize the redis store
            Console.WriteLine("Initialization completed");
            services.AddHttpClient();
            services.AddControllers();
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var exportType = Environment.GetEnvironmentVariable("EXPORT_TYPE") ?? "JAEGER";
            var serviceName = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "CARTSERVICE" + (exportType == "JAEGER" ? string.Empty : $"-{exportType}");
            var myList = buildResourceAsync();


            services.AddOpenTelemetryTracing((builder) => builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, "Resource Detection", null, false, $"{serviceName}-{exportType}-{Guid.NewGuid().ToString()}").AddAttributes(myList))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri("http://10.55.14.225:55680");
                })
                );

        }

        public static IEnumerable<KeyValuePair<string, object>> buildResourceAsync()
        {
            var attributeList = new List<KeyValuePair<string, object>>();
                
                attributeList.Add(new KeyValuePair<string, object>("cloud.region", EC2ResourceDetection.EC2.getAvailabilityZone()));
                Console.WriteLine("cloud.region :" + EC2ResourceDetection.EC2.getAvailabilityZone());
                attributeList.Add(new KeyValuePair<string, object>("cloud.provide", EC2ResourceDetection.EC2.getCloudProvider()));
                Console.WriteLine("cloud.provide" + EC2ResourceDetection.EC2.getCloudProvider());
                attributeList.Add(new KeyValuePair<string, object>("instance.id", EC2ResourceDetection.EC2.getInstanceId()));
                Console.WriteLine("instance.id" + EC2ResourceDetection.EC2.getInstanceId());
                attributeList.Add(new KeyValuePair<string, object>("host.name", EC2ResourceDetection.EC2.getHostName()));
                Console.WriteLine("host.name" + EC2ResourceDetection.EC2.getHostName());
                attributeList.Add(new KeyValuePair<string, object>("instance.type", EC2ResourceDetection.EC2.getInstanceType()));
                Console.WriteLine("instance.type" + EC2ResourceDetection.EC2.getInstanceType());
                attributeList.Add(new KeyValuePair<string, object>("mac.address", EC2ResourceDetection.EC2.getMacAddress()));
                Console.WriteLine("mac.address" + EC2ResourceDetection.EC2.getMacAddress());
                attributeList.Add(new KeyValuePair<string, object>("local.name", EC2ResourceDetection.EC2.getLocalHostName()));
                Console.WriteLine("local.name" + EC2ResourceDetection.EC2.getLocalHostName());
                attributeList.Add(new KeyValuePair<string, object>("host.image.id", EC2ResourceDetection.EC2.getHostImageId()));
                Console.WriteLine("host.image.id" + EC2ResourceDetection.EC2.getHostImageId());
                attributeList.Add(new KeyValuePair<string, object>("account.id", EC2ResourceDetection.EC2.getAccountId()));
                Console.WriteLine("account.id" + EC2ResourceDetection.EC2.getAccountId());
                attributeList.Add(new KeyValuePair<string, object>("cloud.region" ,EC2ResourceDetection.EC2.getRegion()));
                Console.WriteLine("cloud.region" + EC2ResourceDetection.EC2.getRegion());
                attributeList.Add(new KeyValuePair<string, object>("host.id", EC2ResourceDetection.EC2.getARN()));
                Console.WriteLine("host.id"+ EC2ResourceDetection.EC2.getARN());

            return attributeList;
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/java", async context =>
                {
                    await context.Response.WriteAsync("This is response from ASP NET CORE API! <br>" +
                    "  host.name :" + EC2ResourceDetection.EC2.getHostName()+
                    "  cloud.provide :"+ EC2ResourceDetection.EC2.getCloudProvider()+
                    "  host.id :" + EC2ResourceDetection.EC2.getInstanceId()+
                    "  host.type :"+ EC2ResourceDetection.EC2.getInstanceType()+
                    "  mac.address :"+ EC2ResourceDetection.EC2.getMacAddress()+
                    "  host.image.id :"+ EC2ResourceDetection.EC2.getHostImageId()+
                    "  cloud.region :" + EC2ResourceDetection.EC2.getRegion()+
                    "account.id :"+ EC2ResourceDetection.EC2.getAccountId());
                       //"  mac.address :"+ EC2ResourceDetection.EC2.getMacAddress()+
                    //"  host.image.id :"+ EC2ResourceDetection.EC2.getHostImageId()
                    //using (var client = new HttpClient())
                    //{
                     //   var request = new HttpRequestMessage(HttpMethod.Get, "http://10.55.14.142:8080/hello");
                      //  var response = await client.SendAsync(request);
                        //await context.Response.WriteAsync(response.ToString());
                    //}


                });
            });
        }
    }
}
