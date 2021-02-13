using System;
using System.IO;
using System.Text;
using System.Threading;
using Coflnet;
using RestSharp;
using WebSocketSharp.Server;
using WebSocketSharp;
using RestSharp.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
using WebSocketSharp.Net;
using Stripe;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace hypixel
{
    public class Server
    {

        public Server()
        {
        }
        HttpServer server;

        /// <summary>
        /// Starts the backend server
        /// </summary>
        public async Task Start(short port = 8008, string urlPath = "/skyblock")
        {
            server = new HttpServer(port);

            server.AddWebSocketService<SkyblockBackEnd>(urlPath);
            // do NOT timeout after 60 sec
            server.KeepClean = false;
            server.OnGet += (sender, e) =>
            {
                var req = e.Request;
                var res = e.Response;

                var path = req.RawUrl.Split('?')[0];

                if (path == "/" || path.IsNullOrEmpty())
                {
                    path = "index.html";
                }

                if (path == "/stats" || path.EndsWith("/status") || path.Contains("show-status"))
                {
                    PrintStatus(res);
                    return;
                }

                byte[] contents;
                var relativePath = $"files/{path}";

                if (path.StartsWith("/static/skin"))
                {
                    if (!FileController.Exists(relativePath))
                    {
                        // try to get it from mojang
                        var client = new RestClient("https://textures.minecraft.net/");
                        var request = new RestRequest("/texture/{id}");
                        request.AddUrlSegment("id", Path.GetFileName(relativePath));
                        Console.WriteLine(Path.GetFileName(relativePath));
                        var fullPath = FileController.GetAbsolutePath(relativePath);
                        FileController.CreatePath(fullPath);
                        var inStream = new MemoryStream(client.DownloadData(request));

                        //client.DownloadData(request).SaveAs(fullPath+ "f.png" );

                        // parse it to only show face
                        // using (var inStream = new FileStream(File.Open("fullPath",FileMode.Rea)))
                        using (var outputImage = new Image<Rgba32>(16, 16))
                        {
                            var baseImage = SixLabors.ImageSharp.Image.Load(inStream);

                            var lowerImage = baseImage.Clone(
                                            i => i.Resize(256, 256)
                                                .Crop(new Rectangle(32, 32, 32, 32)));

                            lowerImage.Save(fullPath + ".png");

                        }
                        FileController.Move(relativePath + ".png", relativePath);
                    }
                }


                if (!FileController.Exists(relativePath))
                {
                    //res.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
                    //return;
                    // vue.js will handle it internaly
                    relativePath = $"files/index.html";
                }

                try
                {
                    contents = FileController.ReadAllBytes(relativePath);
                }
                catch (Exception)
                {
                    res.WriteContent(Encoding.UTF8.GetBytes("File not found, maybe you fogot to upload the fronted"));
                    return;
                }

                if (path.EndsWith(".html"))
                {
                    res.ContentType = "text/html";
                    res.ContentEncoding = Encoding.UTF8;
                }
                else if (path.EndsWith(".png") || path.StartsWith("/static/skin"))
                {
                    res.ContentType = "image/png";
                    res.ContentEncoding = Encoding.UTF8;
                }
                else if (path.EndsWith(".css"))
                {
                    res.ContentType = "text/css";
                    res.ContentEncoding = Encoding.UTF8;
                }
                else if (path.EndsWith(".js"))
                {
                    res.ContentType = "text/javascript";
                    res.ContentEncoding = Encoding.UTF8;
                }

                res.WriteContent(contents);
            };

            server.OnPost += async (sender, e) =>
            {
                if (e.Request.RawUrl == "/stripe")
                    await ProcessStripe(e);
            };



            server.Start();
            Console.WriteLine("started http");
            //Console.ReadKey (true);
            await Task.Delay(Timeout.Infinite);
            server.Stop();
        }

        private async Task ProcessStripe(HttpRequestEventArgs e)
        {
            Console.WriteLine("received callback from stripe --");

            try
            {
                Console.WriteLine("reading json");
                var json = new StreamReader(e.Request.InputStream).ReadToEnd();
                //Console.WriteLine(e.)

                var stripeEvent = EventUtility.ConstructEvent(
                  json,
                  e.Request.Headers["Stripe-Signature"],
                  Program.StripeSigningSecret
                );
                Console.WriteLine("stripe valiadted");

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    Console.WriteLine("stripe checkout completed");
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    // Fulfill the purchase...
                    await this.FulfillOrder(session);
                } else {
                    Console.WriteLine("sripe  is not comlete type of " + stripeEvent.Type);
                }


                e.Response.StatusCode = 200;
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Ran into exception for stripe callback {ex.Message}");
                e.Response.StatusCode = 400;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ran into an unknown error :/ {ex.Message} {ex.StackTrace}");
            }
        }

        private async Task FulfillOrder(Stripe.Checkout.Session session)
        {
            var googleId = session.ClientReferenceId;
            var id = session.CustomerId;
            var email = session.CustomerEmail;
            var days = Int32.Parse(session.Metadata["days"]);
            Console.WriteLine("STRIPE");
            using (var context = new HypixelContext())
            {
                var user = await context.Users.Where(u => u.GoogleId == googleId).FirstAsync();
                AddPremiumTime(days, user);
                user.Email = email + DateTime.Now;
                context.Update(user);
                await context.SaveChangesAsync();
                Console.WriteLine("order completed");
            }
        }

        public static void AddPremiumTime(int days, GoogleUser user)
        {
            if (user.PremiumExpires > DateTime.Now)
                user.PremiumExpires += TimeSpan.FromDays(days);
            else
                user.PremiumExpires = DateTime.Now + TimeSpan.FromDays(days);
        }

        private static void PrintStatus(HttpListenerResponse res)
        {
            var data = new Stats()
            {
                NameRequests = Program.RequestsSinceStart,
                Indexed = Indexer.IndexedAmount,
                LastIndexFinish = Indexer.LastFinish,
                LastBazaarUpdate = dev.BazaarUpdater.LastUpdate,
                LastNameUpdate = NameUpdater.LastUpdate,
                CacheSize = CacheService.Instance.CacheSize,
                QueueSize = Indexer.QueueCount,
                LastAuctionPull = Updater.LastPull,
                LastUpdateSize = Updater.UpdateSize,
                SubscriptionTobics = SubscribeEngine.Instance.SubCount,
                ConnectionCount = SkyblockBackEnd.ConnectionCount
            };

            // determine status
            res.StatusCode = 200;
            var maxTime = DateTime.Now.Subtract(new TimeSpan(0, 5, 0));
            if (data.LastIndexFinish < maxTime
                || data.LastBazaarUpdate < maxTime
                || data.LastNameUpdate < maxTime
                || data.LastAuctionPull < maxTime)
            {
                res.StatusCode = 500;
            }


            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            res.WriteContent(Encoding.UTF8.GetBytes(json));
        }

        public void Stop()
        {
            server.Stop();
        }
    }
}
