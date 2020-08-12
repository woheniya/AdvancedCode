using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreadLearning
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region Private Method
        /// <summary>
        /// 一个比较耗时耗资源的私有方法
        /// </summary>
        /// <param name="name"></param>
        private void DoSomethingLong(string name)
        {
            Console.WriteLine($"****************DoSomethingLong Start  {name}  {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            long lResult = 0;
            //for (int i = 0; i < 1000_000_000; i++)
            //{
            //    lResult += i;
            //}
            Thread.Sleep(2000); //线程是闲置的；
            Console.WriteLine($"****************DoSomethingLong   End  {name}  {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {lResult}***************");
        }
        #endregion
        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSync_Click(object sender, EventArgs e)
        {
            //基础线程Thread.CurrentThread.ManagedThreadId 对标于每一个线程的一个ID;
            {
                //Console.WriteLine($"****************btnSync_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
                //int l = 3;
                //int m = 4;
                //int n = l + m;
                //for (int i = 0; i < 5; i++)
                //{
                //    string name = string.Format($"btnSync_Click_{i}");
                //    this.DoSomethingLong(name);
                //}
                //Console.WriteLine($"****************btnSync_Click   End {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            }
            //控制线程的调用顺序.无返回值
            {
                //控制线程的调用顺序
                //ThreadStart threadStart = () =>
                //{
                //    Console.WriteLine("你好啊，兄嘚");
                //};
                //Action action = () =>
                //{
                //    Console.WriteLine("你好啊，兄嘚姐妹");
                //};
                //action += () =>
                //{
                //    Console.WriteLine("你好啊");
                //};
                //action += () =>
                //{
                //    Console.WriteLine("Hello");
                //};
                ////包一层大法，不卡界面
                ////Thread thread = new Thread(threadStart);
                //ThreadTest(threadStart, action);
            }
            //控制线程的调用顺序.有返回值
            {
                Func<int> func = () =>
                {
                    Thread.Sleep(5000);
                    return DateTime.Now.Year;
                };

                Func<int> yfun = ThreadHasReturn(func);
                Console.WriteLine(yfun.Invoke());

                //Console.WriteLine(ThreadHasReturn(func));
            }

        }
        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAsync_Click(object sender, EventArgs e)
        {
            //异步初相识
            {
                Console.WriteLine($"****************btnAsync_Click Start {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");

                Action<string> action = this.DoSomethingLong;
                action.Invoke("btnAsync_Click");
                //action("btnAsync_Click");

                action.BeginInvoke("btnAsync_Click", null, null); //会分配一个新的线程去执行；20200803Core还不支持；

                for (int i = 0; i < 5; i++)
                {
                    string name = string.Format($"btnAsync_Click_{i}");
                    action.BeginInvoke(name, null, null);
                }
                Console.WriteLine($"****************btnAsync_Click End   {Thread.CurrentThread.ManagedThreadId.ToString("00")} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}***************");
            }
            
            //回调函数

        }
        //无返回值
        private void ThreadNoReturn(ThreadStart threadStart,Action action)
        {
            ThreadStart thread = () =>
            {
                threadStart.Invoke();
                action.Invoke();
            };

            Thread thread1 = new Thread(thread);
            thread1.Start();
        }
        //有返回值
        private Func<T> ThreadHasReturn<T>(Func<T> func)
        {

            #region 不等待无法得到返回结果
            //T t = default(T);
            //ThreadStart threadStart = () =>
            //{
            //    t = func.Invoke();
            //};
            //Thread thread = new Thread(threadStart);
            //thread.Start();
            //return t;
            #endregion
            T t = default(T);
            ThreadStart threadStart = () =>
            {
                t = func.Invoke();
            };
            Thread thread = new Thread(threadStart);
            thread.Start();
            return new Func<T>(() =>
            {
                thread.Join();
                return t;
            });

            //疑问一
            //thread.Join();
            //return t;


        }
    }
}
