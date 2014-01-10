using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Storage;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using Google.Apis.Storage.v1beta2;
using Google.Apis.Download;


namespace Discovery.ListAPIs
{
    /// <summary>
    /// This example uses the discovery API to list all APIs in the discovery repository.
    /// http://code.google.com/apis/discovery/v1/using.html
    /// </summary>
    class Program
    {

        bool isStarting = true;

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Discovery API Sample");
            Console.WriteLine("====================");

            try
            {
                new Program().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private async Task Run()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { Google.Apis.Storage.v1beta2.StorageService.Scope.DevstorageFullControl },
                    "user", CancellationToken.None);
            }


            // Create the service.
            var service = new StorageService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Discovery Sample"
            });

            // Run the request.
            Console.WriteLine("Executing a list request...");
            var result = service.Objects.Get("helm", "IMG_0226.MOV");

            // Display the results.
            if (result != null)
            {
                // Get the client request object for the bucket and desired object
                using (var fileStream = new System.IO.FileStream("C:\\Img_0226.MOV",
                  System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    // Add an handler which will be notified on progress changes
                    result.MediaDownloader.ProgressChanged += Download_ProgressChanged;
                    await result.DownloadAsync(fileStream);
                    
                }
            }
        }

        private void Download_ProgressChanged(IDownloadProgress progress)
        {
            if (isStarting == true)
            {
                System.Diagnostics.Process.Start("C:\\Img_0226.MOV");
                isStarting = false;
            }
          Console.WriteLine(progress.Status + " " + progress.BytesDownloaded);
        }
    }
}

