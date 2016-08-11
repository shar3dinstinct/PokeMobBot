using PoGo.PokeMobBot.Logic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoGo.PokeMobBot.Logic.State;

namespace aGUI
{
    public class GUILogger : ILogger
    {
        private readonly LogLevel _maxLogLevel;
        private ISession _session;

        internal GUILogger(LogLevel maxLogLevel)
        {
            _maxLogLevel = maxLogLevel;
        }

        public void SetSession(ISession session)
        {
            throw new NotImplementedException();
        }

        public void Write(string message, LogLevel level = LogLevel.Info, ConsoleColor color = ConsoleColor.Black)
        {
            throw new NotImplementedException();
        }
    }
}
