using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXILED;

namespace DCReplace
{
    internal static class MsgConfig
    {
        internal static string ReplaceCustomMsg = Plugin.Config.GetString("dcr_message", "<i>You have replaced a disconnected player</i>");
    }
}
