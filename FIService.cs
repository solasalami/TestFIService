using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.Extensions.Configuration;

namespace TestFIService
{
	public class FIService
	{
        private IConfiguration _configuration;

        public FIService(IConfiguration configuration)
		{
            _configuration = configuration;
		}

        public static bool TrustAllCertificateCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {

            return true;

        }

        [Obsolete]
        public string InvokeService(string XMLData)
        {
            string serviceResponse = string.Empty;

            try
            {
                //Trust All Certificates
                ServicePointManager.ServerCertificateValidationCallback = TrustAllCertificateCallback;

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12;

                //Calling CreateSOAPWebRequest method  
                HttpWebRequest request = CreateSOAPWebRequest();
                XmlDocument SOAPReqBody = new XmlDocument();
                //SOAP Body Request  
                SOAPReqBody.LoadXml(XMLData);
                using (Stream stream = request.GetRequestStream())
                {
                    SOAPReqBody.Save(stream);
                }
                //Geting response from request  
                using (WebResponse Serviceres = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                    {

                        var ServiceResult = rd.ReadToEnd();
                        serviceResponse = ServiceResult.Trim();
                        Console.WriteLine("Response / Invoke Webservice XML ==> " + serviceResponse + "     --------@ " + DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in SOAP Webservice InvokeService " + ex.ToString() + " @ " + DateTime.Now);
            }



            return serviceResponse.Trim();
        }

        [Obsolete]
        private HttpWebRequest CreateSOAPWebRequest()
        {
            //Make Web Request  
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(_configuration["FinacleSoapURL"]);
            //SOAP Action  
            Req.Headers.Add("SOAPAction:" + _configuration["FinacleSoapWSDL"]);
            //Content_type  
            Req.ContentType = "text/xml;charset=\"utf-8\"";
            Req.Accept = "text/xml";
            //HTTP method  
            Req.Method = "POST";
            //return HttpWebRequest  
            return Req;
        }

    }
}

