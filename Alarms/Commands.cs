using PulsarPluginLoader.Chat.Commands;
using PulsarPluginLoader.Utilities;

namespace Alarms
{
    class Commands : IChatCommand
    {
        public string[] CommandAliases()
        {
            return new string[] { "alarms", "alarm" };
        }

        public string Description()
        {
            return "controls the behavior of the Alarms plugin";
        }

        public bool Execute(string arguments, int SenderID)
        {
            string[] args = arguments.Split(' ');
            bool yes = false;
            float woah = -1;
            if (args.Length > 1)
            {
                yes = float.TryParse(args[1], out woah);

            }


            switch (args[0].ToLower())
            {
                case "hull":
                    if (yes)
                    {
                        Global.hull = woah;
                        Messaging.Notification($"Hull value set to {woah}");
                    }
                    else
                    {
                        Messaging.Notification($"Argument not found, current value is {Global.hull}");
                    }
                    break;
                case "shield":
                    if (yes)
                    {
                        Global.shield = woah;
                        Messaging.Notification($"Shield value set to {woah}");
                    }
                    else
                    {
                        Messaging.Notification($"Argument not found, current value is {Global.shield}");
                    }
                    break;
                case "o2":
                    if (yes)
                    {
                        Global.o2 = woah;
                        Messaging.Notification($"Oxygen value set to {woah}");
                    }
                    else
                    {
                        Messaging.Notification($"Argument not found, current value is {Global.o2}");
                    }

                    break;
                case "firecount":
                    if (yes)
                    {
                        Global.firecount = (int)woah;
                        Messaging.Notification($"Fire number set to {woah}");
                    }
                    else
                    {
                        Messaging.Notification($"Argument not found, current value is {Global.firecount}");
                    }
                    break;
                case "reset":
                    {
                        Global.hull = .40f;
                        Global.shield = .98f;
                        Global.o2 = .15f;
                        Global.firecount = 15;
                        PLXMLOptionsIO.Instance.CurrentOptions.SetStringValue("AlarmSettings", $"{Global.hull} {Global.shield} {Global.o2} {Global.firecount}");
                        Messaging.Notification("All values set to defaults!");
                    }
                    break;
                case "values":
                    {
                        Messaging.Notification($"Hull %: {Global.hull}\nShield %: {Global.shield}\nOxygen %: {Global.o2}\nFires: {Global.firecount}");
                    }
                    break;
                default:
                    Messaging.Notification("Subcommand not found");
                    break;
            }
            if(yes)
            {
                PLXMLOptionsIO.Instance.CurrentOptions.SetStringValue("AlarmSettings", $"{Global.hull} {Global.shield} {Global.o2} {Global.firecount}");
            }
            return false;
        }

        public bool PublicCommand()
        {
            return false;
        }

        public string UsageExample()
        {
            return "/alarms [hull | shield | o2 | firecount | reset | values]";
        }
    }
}
