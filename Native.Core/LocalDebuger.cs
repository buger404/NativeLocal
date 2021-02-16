#define LOCAL_DEBUG

using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;

namespace Native.Core
{
	public class LocalDebug
    {
		// 机器人看到的消息的群号码
		public static long GroupID;
		// 机器人看到的消息的QQ号
		public static long QQID;

		public static IUnityContainer FakeCoolQ = new UnityContainer();
		public static ICQStartup Startup;
		public static IGroupMessage GroupMessage;
		public static CQApi CQApi;
		public static CQLog CQLog;
		public static int msgid;

		public static void Launch()
		{
#if LOCAL_DEBUG
#else
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("~该dll已经正常编译，不可启动调试。~");
			Console.ReadLine();
			return;
#endif
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("~机器人本地测试工具~");
			Console.WriteLine("特殊指令：\n%setqq <qq> 设置测试用的QQ号\n%setgroup <group> 设置测试用的群号\n");
			Console.Write("输入测试用的QQ号：");
			QQID = long.Parse(Console.ReadLine());
			Console.Write("输入测试用的群号：");
			GroupID = long.Parse(Console.ReadLine());
			//传递假的container
			CQMain.Register(FakeCoolQ);
			//注册假的CQ
			CQApi = new CQApi(new Sdk.Cqp.Model.AppInfo("local_debuger",0,9,"local_debuger","1.0.0",10,"","",404233));
			CQLog = new CQLog(404233);
			//取得注册的接口
			Startup = FakeCoolQ.Resolve<ICQStartup>("酷Q启动事件");
			GroupMessage = FakeCoolQ.Resolve<IGroupMessage>("群消息处理");
			//触发假的启动事件
			Startup.CQStartup(null, new CQStartupEventArgs(CQApi, CQLog, 0, 0, "startup", "CQStartup", 0));

			//触发假的群聊事件
			while (true)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("QQ" + QQID + "@" + GroupID + ">");
				string content = Console.ReadLine();
				if (content.StartsWith("%"))
				{
					string[] p = content.Split(' ');
					if (p[0] == "%setqq") QQID = long.Parse(p[1]);
					if (p[0] == "%setgroup") GroupID = long.Parse(p[1]);
				}
				else
				{
					GroupMessage.GroupMessage(null, new CQGroupMessageEventArgs(CQApi, CQLog, 0, 0, "groupmessage", "CQGroupMessage", 0, 0,
						msgid, GroupID, QQID, "", content, false));
					msgid++;
				}
			}

		}
	}
}
