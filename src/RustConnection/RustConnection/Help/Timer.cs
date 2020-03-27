using System;
using RustConnection.Base;
using RustConnection.Manager;

namespace RustConnection.Help
{
    public class Timer : IWorker
    {
        public static Timer Once(Action handler, float timeout)
        {
            Timer timer = AppManager.AddWorker<Timer>();
            timer.Handler = handler;
            timer.TotalSeconds = timeout;

            return timer;
        }
        
        public static Timer Repeat(Action handler, float timeout)
        {
            Timer timer = AppManager.AddWorker<Timer>();
            timer.Handler = handler;
            timer.TotalSeconds = timeout;
            timer.CanRepeat = true;

            return timer;
        }
        
        private Action Handler { get; set; }
        private float TotalSeconds { get; set; } = 0f;
        private float Ticker { get; set; } = 0f;
        private bool CanRepeat { get; set; } = false;
        public bool HasDestroy { get; private set; } = false;

        public void Awake()
        {
            
        }

        private void Handle()
        {
            try
            {
                this.Handler.Invoke();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Exception in {this.Handler.Method.DeclaringType}.{this.Handler.Method}(): ");
                Console.WriteLine(ex);
                Console.ResetColor();
            }
        }

        public void Destroy(bool useHandle = false)
        {
            if (this.HasDestroy == true)
            {
                return;
            }

            this.HasDestroy = true;
            
            if (useHandle == true)
            {
                this.Handle();
            }
            AppManager.Destroy(this);
        }

        public void Update(float deltaTime)
        {
            if (this.HasDestroy == true)
            {
                return;
            }
            
            this.Ticker += deltaTime;
            if (this.Ticker >= this.TotalSeconds)
            {
                this.Handle();

                if (this.CanRepeat == false)
                {
                    this.Destroy();
                }
                else
                {
                    this.Ticker = 0;
                }
            }
        }
    }
}