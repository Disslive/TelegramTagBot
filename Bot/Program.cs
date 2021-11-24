using System;
using System.Data.SQLite;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
namespace Bot
{
    class Program
    {
            private static ITelegramBotClient botClient;
            public static string[] commands = {"/start", "/help", "/all", "/active", "/inactive", "/group_create", 
            "/group_members_add", "/group_members_delete", "/group_delete", "/grouptag", "/group_leave",
            "/my_groups", "/group_member" };
   
        static void Main(string[] args)
        {

            string token = File.ReadAllText(@"token.txt"); ;

            botClient = new TelegramBotClient(token) {Timeout = TimeSpan.FromSeconds(10)};

            var me = botClient.GetMeAsync().Result;

            Console.WriteLine($"id : {me.Id} , name : {me.FirstName}, user : {me.Username}");
         
            botClient.OnMessage += MessageReact;
           
            botClient.StartReceiving();

            Console.ReadKey();
            
        }

       
        private static async void MessageReact(object user, MessageEventArgs e)
        {

           
                var text = e?.Message?.Text;
                var id = e.Message.Chat.Id;
                var username = e.Message.From.Username; 
                string answer = "";
                DataBaseInteract dbi = new DataBaseInteract();
                GroupUsers gu = new GroupUsers();
                if (text == null)
                    return;
                var lowtext = text.ToLower();
                 dbi.Insert($"INSERT INTO users(chat_id, Active, username) VALUES ({id}, 1, '@{username}')");

                switch (lowtext)
                {
                    case var g when lowtext.StartsWith(commands[0]):
                    answer = $"Hello, to get all information to work with me: /help ";
                    break;
                    case var g when lowtext.Contains(commands[1]):
                    answer = File.ReadAllText(@"help.txt");
                    answer = answer.Replace("MARKFORUSERNAME", $"@{username}");
                    break;
                    case var g when lowtext.Contains(commands[2]):
                    SQLiteDataReader reader = dbi.Select($"SELECT username from users WHERE Active = 1 AND chat_id = {id}");
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                answer += $"{reader.GetValue(0)} ";
                            }
                        }
                        else
                        {
                            answer = "All users are inactive.";
                        }
                        break;
                    case var g when lowtext.Contains(commands[3]):
                    dbi.Insert($"INSERT INTO users(chat_id, Active, username) VALUES ({id}, 1, '@{username}')");
                        dbi.Update($"UPDATE users set Active = 1 WHERE username = '@{username}'");
                        answer = "Now you an active user, be ready to play (/inactive and bot will not tag you).";
                        break;
                    case var g when lowtext.Contains(commands[4]):
                        dbi.Insert($"INSERT INTO users(chat_id, Active, username) VALUES ({id}, 0, '@{username}')");
                        dbi.Update($"UPDATE users set Active = 0 WHERE username = '@{username}'");
                        answer = "Now you an inactive user (/active and bot will tag you).";
                        break;
                    case string g when lowtext.StartsWith(commands[5]):
                        g = ReplaiceCommand(g);
                        gu.GroupCreate($"@{username}", g, id);
                        answer = $"group {g} is created";
                        break;
                    case string g when lowtext.StartsWith(commands[6]):
                        g = ReplaiceCommand(g);
                        string[] subs = g.Split(' ', ',');
                        answer = $"users added to group {subs[1]}";
                        gu.GroupUsersInteract(subs, id, username, 1);
                        break;
                    case string g when lowtext.StartsWith(commands[7]):
                        g = ReplaiceCommand(g);
                          string[] delsubs = g.Split(' ', ',');
                        answer = $"users deleted from group {delsubs[1]}";
                        gu.GroupUsersInteract(delsubs, id, username, 2);
                        break;
                    case string g when lowtext.StartsWith(commands[8]):
                        g = ReplaiceCommand(g);
                        gu.GroupDelete(g, id, username);
                        answer = $"group {g} deleted";
                        break;
                    case string g when lowtext.StartsWith(commands[9]):
                        g = ReplaiceCommand(g);
                        answer = gu.GroupTag(g, id);
                        break;
                    case string g when lowtext.StartsWith(commands[10]):
                        g = ReplaiceCommand(g);
                        gu.GroupLeave(g, id, username);
                        answer = $"You leaved from group {g}";
                        break;
                    case var g when lowtext.StartsWith(commands[11]):
                        answer = gu.MyGroupList(id, username);
                        break;
                    case var g when lowtext.StartsWith(commands[12]):
                        answer = gu.GroupsMember(id, username);
                        break;
                }
                try {
                    if (answer != "")
                    {
                        Console.WriteLine($"Bot received command {text} in chat {id} from @{username}");
                        await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: answer

                            ).ConfigureAwait(false);
                    } } catch (Telegram.Bot.Exceptions.ApiRequestException) {
                Console.WriteLine("ApiRequestException: group became supergroup");
                };
           
        } 

        public static string ReplaiceCommand(string answer)
        {
            
            if (answer.Contains(commands[6]) == false && answer.Contains(commands[7]) == false) answer = answer.Replace(" ", "");

            foreach (string x in commands)
            {
                if (answer.Contains(x)) {
                answer = answer.Replace($"{x}", "");
                answer = answer.Replace("@dskjgfdkjsh_bot", "");
                }
            }

            return answer;
        }

    }
}
