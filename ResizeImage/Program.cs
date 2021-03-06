﻿using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;

namespace ResizeImage
{
    class Program
    {
        private static string BytesToSrcString(byte[] bytes) => "data:image/png;base64," + Convert.ToBase64String(bytes);
        private static Uri url = new Uri("https://api.projectoxford.ai/vision/v1.0/generateThumbnail?height=200&width=200&smartCropping=true");
        private static string _apiKey = "<YOUR_API_KEY>";
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            httpC();
        }

        static void httpC()
        {
            using (var httpClient = new HttpClient())
            {
                //setup HttpClient
                httpClient.BaseAddress = url;
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
                httpClient.DefaultRequestHeaders.Add("User-Agent","DotNetClient");
                //httpClient.DefaultRequestHeaders.Add("MediaType", "multipart/form-data");

                //setup data object
                var listofFiles = Directory.EnumerateFiles(@"C:\Users\224003088\Pictures\iPhone\DCIM\961VCBLL", "*.JPG");
                int xCounter = 1;
                foreach(var file in listofFiles)
                {
                Console.WriteLine(file);
                var _outFile = @"c:\temp\"+ xCounter.ToString() + ".PNG";
                xCounter++;
                
                if(xCounter % 20 == 0)
                {
                        //There is a governor limit of 20 images per minute. So making the process sleep.
                        System.Threading.Thread.Sleep(60000);
                }
                    HttpContent content = new StreamContent(readImageBytes(file));
                //HttpContent content = new StreamContent(readImageBytes(@"C:\Users\224003088\Pictures\iPhone\IMG_8689.JPG"));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                //make request
                var response = httpClient.PostAsync(url, content);

                //read response and write to view data
                var responseContent = response.Result.Content.ReadAsByteArrayAsync();
                //Console.WriteLine(Convert.ToBase64String(responseContent.Result));
                //Console.WriteLine(Convert.ToString(responseContent.Result.ToString()));
                Console.WriteLine(BytesToSrcString(responseContent.Result));
                FileStream _FileStream = new FileStream(_outFile, FileMode.Create, System.IO.FileAccess.Write);
                _FileStream.WriteAsync(responseContent.Result, 0, responseContent.Result.Length);
                _FileStream.Close();

                }
            }
        }

        static Stream readImageBytes(string filePath)
        {
            if (File.Exists(filePath))
            {
                return new MemoryStream(File.ReadAllBytes(filePath));
            }
            else
            {
                Console.WriteLine("File {0} is not available.",filePath);
                return null;
            }
        }
    }
}
