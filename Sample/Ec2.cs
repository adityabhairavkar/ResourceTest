using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EC2ResourceDetection
{
    public static class EC2
    {
        public static string getHostName()
        {
            return Amazon.Util.EC2InstanceMetadata.Hostname;
        }
        //return host Instance Id for ec2 instance
        public static string getInstanceId()
        {

            return Amazon.Util.EC2InstanceMetadata.InstanceId;
        }
        //return host InstanceType for ec2 instance
        public static string getInstanceType()
        {

            return Amazon.Util.EC2InstanceMetadata.InstanceType;
        }

        internal static object getCloudProvider()
        {
            return "AWS";
        }

        //return host LocalHost Name for ec2 instance
        public static string getLocalHostName()
        {

            return Amazon.Util.EC2InstanceMetadata.LocalHostname;
        }
        //return host Mac Address for ec2 instance
        public static string getMacAddress()
        {

            return Amazon.Util.EC2InstanceMetadata.MacAddress;
        }
        //return host Availability Region for ec2 instance
        public static string getAvailabilityZone()
        {

            return Amazon.Util.EC2InstanceMetadata.AvailabilityZone;
        }

        public static string getHostImageId()
        {

            return Amazon.Util.EC2InstanceMetadata.AmiId;
        }

        public static string getAccountId()
        {
            string id = Amazon.Util.EC2InstanceMetadata.IdentityDocument;
            string[] data = id.Split(",");
            data = data[0].Split(":");
            return data[1];
        }
        public static async Task<bool> isEC2Instance()
        {
            try
            {

                var response = await pingEC2();
                Console.WriteLine("Response" + response + " " + response.StatusCode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception :" + e);
                return false;

            }
            return true;


        }
        public static async Task<HttpResponseMessage> pingEC2()
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://169.254.169.254/");
                client.Timeout = TimeSpan.FromSeconds(10);
                var response = await client.SendAsync(request);
                return response;
            }
        }

    }
}