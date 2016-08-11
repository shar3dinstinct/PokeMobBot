using System;
using System.Globalization;
using System.Threading;
using PoGo.PokeMobBot.Logic;
using PoGo.PokeMobBot.Logic.Common;
using PoGo.PokeMobBot.Logic.Event;
using PoGo.PokeMobBot.Logic.Logging;
using PoGo.PokeMobBot.Logic.State;
using PoGo.PokeMobBot.Logic.Tasks;
using PoGo.PokeMobBot.Logic.Utils;
using System.Windows.Forms;

namespace aGUI
{
    public partial class BotForm : Form
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        public BotForm()
        {
            InitializeComponent();

            var culture = CultureInfo.CreateSpecificCulture("en-US");

            CultureInfo.DefaultThreadCurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            //var subPath = "";
            //if (args.Length > 0)
            //    subPath = args[0];
#if DEBUG
            LogLevel logLevel = LogLevel.Debug;
#else
            LogLevel logLevel = LogLevel.Info;
#endif
            //Logger.SetLogger(new ConsoleLogger(logLevel), subPath);


        }
    }
}
