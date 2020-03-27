using System;
using System.Net;
using System.Text;
using System.Threading;
using RustConnection.Base;
using RustConnection.Manager;

namespace RustConnection.Help
{
    public class WebRequest : IWorker
    {
        public static WebRequest GetWebRequest(string url, Action<string, bool, Exception> handler)
        {
            WebRequest webRequest = AppManager.AddWorker<WebRequest>();
            webRequest.Handler = handler;
            webRequest.Url = url;

            return webRequest;
        }

        private string Url { get; set; } = null;
        private Action<string, bool, Exception> Handler { get; set; } = null;
        public bool HasDestroy { get; private set; } = false;
        private Thread RequestThread { get; set; } = null;

        public void Awake()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                this.RequestThread = Thread.CurrentThread;
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    try
                    {
                        string content = wc.DownloadString(this.Url);
                        this.OnResult(content, true, null);
                    }
                    catch (Exception ex)
                    {
                        this.OnResult(null, false, ex);
                    }
                }
            });
        }

        public void Destroy()
        {
            if (this.HasDestroy == true)
            {
                return;
            }
            this.HasDestroy = true;
            if (this.RequestThread != null)
            {
                this.RequestThread.Abort();
                this.RequestThread = null;
            }
            AppManager.Destroy(this);
        }

        private void OnResult(string content, bool result, Exception exception)
        {
            if (this.HasDestroy == true)
            {
                return;
            }
            
            AppManager.CallInMainThread(() =>
            {
                if (this.HasDestroy == true)
                {
                    return;
                }
                this.Destroy();
                
                try
                {
                    this.Handler.Invoke(content, result, exception);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Exception in {this.Handler.Method.DeclaringType}.{this.Handler.Method}(): ");
                    Console.WriteLine(ex);
                    Console.ResetColor();
                }
            });
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}