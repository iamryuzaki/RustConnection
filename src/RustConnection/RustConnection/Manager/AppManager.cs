using System;
using System.Collections.Generic;
using System.Threading;
using Facepunch;
using RustConnection.Base;

namespace RustConnection.Manager
{
    public class AppManager : IWorker
    {
        public static AppManager Instance { get; private set; }
        public static List<IWorker> ListActiveWorkers { get; } = new List<IWorker>();
        public int DelayMilliseconds { get; set; } = 10;
        public bool IsWork { get; set; } = true;
        private Queue<Action> ListActiveTaskInThread = new Queue<Action>();

        public static void CallInMainThread(Action action)
        {
            lock (Instance.ListActiveTaskInThread)
            {
                Instance.ListActiveTaskInThread.Enqueue(action);
            }
        }

        public static void Destroy(IWorker worker)
        {
            lock (ListActiveWorkers)
            {
                if (ListActiveWorkers.Contains(worker))
                {
                    ListActiveWorkers.Remove(worker);
                }
            }
        }
        
        public static T AddWorker<T>() where T : IWorker
        {
            try
            {
                T worker = (T) Activator.CreateInstance(typeof(T), true);
                worker.Awake();
                lock (ListActiveWorkers)
                {
                    ListActiveWorkers.Add(worker);
                }
                return worker;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Exception in {typeof(T)}.Awake() or CreateInstance: ");
                Console.WriteLine(ex);
                Console.ResetColor();
            }

            return default(T);
        }

        public static void Init<T>() where T : IWorker
        {
            AppManager.AddWorker<AppManager>();
            AppManager.AddWorker<T>();

            DateTime tickStart = DateTime.Now;
            while (Instance.IsWork)
            {
                List<Action> listActionsNow = null;
                lock (Instance.ListActiveTaskInThread)
                {
                    while (Instance.ListActiveTaskInThread.Count != 0 && Instance.IsWork)
                    {
                        if (listActionsNow == null)
                        {
                            listActionsNow = Pool.GetList<Action>();
                        }
                        listActionsNow.Add(Instance.ListActiveTaskInThread.Dequeue());
                    }
                }

                if (listActionsNow != null)
                {
                    for (var i = 0; i < listActionsNow.Count && Instance.IsWork; i++)
                    {
                        try
                        {
                            listActionsNow[i].Invoke();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"Exception in {listActionsNow[i].Method.DeclaringType}.{listActionsNow[i].Method}(): ");
                            Console.WriteLine(ex);
                            Console.ResetColor();
                        }
                    }
                    Pool.FreeList(ref listActionsNow);
                }

                for (var i = 0; i < ListActiveWorkers.Count && Instance.IsWork; i++)
                {
                    try
                    {
                        ListActiveWorkers[i].Update((float)DateTime.Now.Subtract(tickStart).TotalSeconds);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Exception in {ListActiveWorkers[i].GetType()}.Update(): ");
                        Console.WriteLine(ex);
                        Console.ResetColor();
                    }
                }
                tickStart = DateTime.Now;
                Thread.Sleep(Instance.DelayMilliseconds);
            }
        }

        public void Awake()
        {
            Instance = this;
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}