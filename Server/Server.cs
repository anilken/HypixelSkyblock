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
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using RateLimiter;

namespace hypixel
{
    public class Server
    {

        public Server()
        {
            Limiter = new IpRateLimiter(ip =>
            {
                var constraint = new CountByIntervalAwaitableConstraint(10, TimeSpan.FromSeconds(1));
                var constraint2 = new CountByIntervalAwaitableConstraint(30, TimeSpan.FromSeconds(10));
                var heavyUsage = new CountByIntervalAwaitableConstraint(100, TimeSpan.FromMinutes(1));

                // Compose the two constraints
                return TimeLimiter.Compose(constraint, constraint2, heavyUsage);
            });
        }
        HttpServer server;

        ConcurrentDictionary<string, int> ConnectionToGoogleId = new ConcurrentDictionary<string, int>();
        private IpRateLimiter Limiter;

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
                AnswerGetRequest(e);
            };

            server.OnPost += async (sender, e) =>
            {
                if (e.Request.RawUrl == "/stripe")
                    await new StripeRequests().ProcessStripe(e);
                if (e.Request.RawUrl.StartsWith("/command/"))
                    await HandleCommand(e.Request, e.Response);
            };

            server.Start();
            Console.WriteLine("started http");
            //Console.ReadKey (true);
            await Task.Delay(Timeout.Infinite);
            server.Stop();
        }

        private void AnswerGetRequest(HttpRequestEventArgs e)
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

            if (path == "/players")
            {
                PrintPlayers(req, res);
                return;
            }
            if (path == "/items")
            {
                PrintItems(req, res);
                return;
            }

            if (path == "/api/items/bazaar")
            {
                PrintBazaarItems(req, res);
                return;
            }
            if (path == "/api/items/search")
            {
                SearchItems(req, res);
                return;
            }

            if (path.StartsWith("/command/"))
            {
                HandleCommand(req, res);
                return;
            }

            byte[] contents;
            var relativePath = $"files/{path}";

            if (path.StartsWith("/static/skin"))
            {
                GetSkin(relativePath);
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

            if (relativePath == "files/index.html")
            {
                Console.WriteLine("is index");
                string response = HtmlModifier.ModifyContent(path, contents);
                if (!response.StartsWith('<'))
                {
                    res.Redirect(response);
                }
                contents = Encoding.UTF8.GetBytes(response);
            }


            res.ContentType = "text/html";
            res.ContentEncoding = Encoding.UTF8;

            if (path.EndsWith(".png") || path.StartsWith("/static/skin"))
            {
                res.ContentType = "image/png";
            }
            else if (path.EndsWith(".css"))
            {
                res.ContentType = "text/css";
            }
            else if (path.EndsWith(".js"))
            {
                res.ContentType = "text/javascript";
            }

            res.WriteContent(contents);
        }




        private async Task HandleCommand(HttpListenerRequest req, HttpListenerResponse res)
        {
            HttpMessageData data = new HttpMessageData(req, res);
            try
            {
                var conId = req.Headers["ConId"].Truncate(32);
                if (conId == null | conId.Length < 32)
                    throw new CoflnetException("invalid_conid", "The 'ConId' Header has to be at least 32 characters long and generated randomly");

                data.SetUserId = id =>
                {
                    this.ConnectionToGoogleId.TryAdd(conId, id);
                };

                if (ConnectionToGoogleId.TryGetValue(conId, out int userId))
                    data.UserId = userId;

                if(data.Type == "test")
                {
                    Console.WriteLine(req.RemoteEndPoint.Address.ToString());
                    foreach (var item in req.Headers.AllKeys)
                    {
                        Console.WriteLine($"{item.ToString()}: {req.Headers[item]}");
                    }
                    return;
                }

                if (CacheService.Instance.TryFromCache(data))
                    return;

                await Limiter.WaitUntilAllowed(req.RemoteEndPoint.Address.ToString());

                if (SkyblockBackEnd.Commands.TryGetValue(data.Type, out Command command))
                    command.Execute(data);
                else
                    throw new CoflnetException("unkown_command", "Command not known, check the docs");
            }
            catch (CoflnetException ex)
            {
                res.StatusCode = 400;
                data.SendBack(new MessageData("error", JsonConvert.SerializeObject(new { ex.Slug, ex.Message })));
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                dev.Logger.Instance.Error($"Fatal error on Command {JsonConvert.SerializeObject(data)} {ex.Message} {ex.StackTrace}");
                data.SendBack(new MessageData("error", JsonConvert.SerializeObject(new { Slug = "unknown", Message = "An unexpected error occured, make sure the format of Data is correct" })));
            }
        }



        private static void GetSkin(string relativePath)
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

        private static async void PrintBazaarItems(HttpListenerRequest req, HttpListenerResponse res)
        {
            var data = await ItemDetails.Instance.GetBazaarItems();


            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data.Select(i => new { i.Name, i.Tag, i.MinecraftType, i.IconUrl }));
            res.WriteContent(Encoding.UTF8.GetBytes(json));
        }

        private static async void PrintPlayers(HttpListenerRequest req, HttpListenerResponse res)
        {
            using (var context = new HypixelContext())
            {
                var data = context.Players.OrderByDescending(p => p.UpdatedAt).Select(p => new { p.Name, p.UuId }).Take(10000);
                StringBuilder builder = GetSiteBuilder("Player");
                foreach (var item in data)
                {
                    if (item.Name == null)
                        continue;
                    builder.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", $"/player/{item.UuId}/{item.Name}", $"{item.Name} auctions");
                }
                res.WriteContent(Encoding.UTF8.GetBytes(builder.ToString()));
            }
        }

        private static async void PrintItems(HttpListenerRequest req, HttpListenerResponse res)
        {
            using (var context = new HypixelContext())
            {
                var data = context.Items.Select(p => new { p.Tag }).Take(10000);
                StringBuilder builder = GetSiteBuilder("Item");
                foreach (var item in data)
                {
                    var name = ItemDetails.TagToName(item.Tag);
                    builder.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", $"/item/{item.Tag}/{name}", $"{name} auctions");
                }
                res.WriteContent(Encoding.UTF8.GetBytes(builder.ToString()));
            }
        }

        private static StringBuilder GetSiteBuilder(string topic)
        {
            var builder = new StringBuilder(20000);
            builder.Append("<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"/>");
            builder.Append($"<link rel=\"icon\" href=\"/favicon.ico\"/><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"/><title>{topic} List</title><body>");
            builder.Append($"<h1>List of the most recently updated {topic}s</h1><a href=\"https://sky.coflnet.com\">back to the start page</a><ul>");
            return builder;
        }

        private static async void SearchItems(HttpListenerRequest req, HttpListenerResponse res)
        {
            var term = req.QueryString["term"];
            Console.WriteLine("searchig for:");
            Console.WriteLine(term);
            var data = await ItemDetails.Instance.Search(term);


            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            res.WriteContent(Encoding.UTF8.GetBytes(json));
        }

        public void Stop()
        {
            server.Stop();
        }
    }
}
