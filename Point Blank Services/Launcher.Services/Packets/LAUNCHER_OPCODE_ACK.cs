using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Server.Network
{
    public enum LAUNCHER_OPCODE_ACK : ushort
    {
        LAUNCHER_CONNECT_REQ  = 1000,
        LAUNCHER_CONNECT_ACK  = 1001,
        LAUNCHER_CONNECT_FAIL = 1002,

        LAUNCHER_MANIFEST_REQ  = 2000,
        LAUNCHER_MANIFEST_ACK  = 2001,
        LAUNCHER_MANIFEST_FAIL = 2002,

        LAUNCHER_FILE_REQ      = 3000,
        LAUNCHER_FILE_INFO_ACK = 3001,
        LAUNCHER_FILE_DATA_ACK = 3002,
        LAUNCHER_FILE_END_ACK  = 3003,
        LAUNCHER_FILE_FAIL     = 3004,

        LAUNCHER_LOGIN_REQ  = 4000,
        LAUNCHER_LOGIN_ACK  = 4001,
        LAUNCHER_LOGIN_FAIL = 4002,

        LAUNCHER_ERROR_ACK = 9000
    }
}