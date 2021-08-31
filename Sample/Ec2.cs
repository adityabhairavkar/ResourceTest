using System;
using System.Net.Http;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;

namespace EC2ResourceDetection
{
    public static class EC2
    {

        public static string ARN, region, accountId, instanceid,platform;
        public static string getHostName()
        {
            return Amazon.Util.EC2InstanceMetadata.Hostname;
        }
        //return host Instance Id for ec2 instance
        public static string getInstanceId()
        {

            instanceid= Amazon.Util.EC2InstanceMetadata.InstanceId;
            if (instanceid != null)
                return instanceid;
            return null;
        }
        //return host InstanceType for ec2 instance
        public static string getInstanceType()
        {

            return Amazon.Util.EC2InstanceMetadata.InstanceType;
        }

        internal static object getCloudProvider()
        {
            platform = "aws:ec2";
            return platform;
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

        public static string getRegion()
        {

            var identityDocument = Amazon.Util.EC2InstanceMetadata.IdentityDocument;
            if (!string.IsNullOrEmpty(identityDocument))
            {
                try
                {
                    var jsonDocument = JsonMapper.ToObject(identityDocument.ToString());
                    region = jsonDocument["region"].ToString();
                    if (region != null)
                        return region;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e + "Error attempting to read region from instance metadata identity document");
                }
            }

            return null;
        }
    
        public static string getHostImageId()
        {

            return Amazon.Util.EC2InstanceMetadata.AmiId;
        }

        public static string getAccountId()
        {
            var identityDocument = Amazon.Util.EC2InstanceMetadata.IdentityDocument;
            if (!string.IsNullOrEmpty(identityDocument))
            {
                try
                {
                    var jsonDocument = JsonMapper.ToObject(identityDocument.ToString());
                    accountId = jsonDocument["accountId"].ToString();
                    if (accountId != null)
                        return accountId;
                }

                catch (Exception e)
                {
                    Console.WriteLine(e + "Error attempting to read region from instance metadata identity document");
                }
            }

            return null;
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
        public static string getARN()
        {
            if(platform!=null && region != null && accountId!=null && instanceid!=null)
            return "arn:" + platform + ":" + region + ":" + accountId + ":instance/" + instanceid;
            return null;
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