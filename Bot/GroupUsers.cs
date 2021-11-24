using System.Data.SQLite;

namespace Bot
{
    class GroupUsers
    {
        DataBaseInteract dbi = new DataBaseInteract();
        SQLiteDataReader reader;
        public void GroupCreate(string creator, string group_name, long chat_id)
        {
            group_name = group_name.Replace(" ", "");
            dbi.Insert($"INSERT INTO groups(chat_id, group_name, creator_username) VALUES ({chat_id}, '{group_name}', '{creator}')");
            dbi.Insert($"INSERT INTO members(chat_id, group_name, member) VALUES ({chat_id}, '{group_name}', '{creator}')");
        }
        public void GroupUsersInteract(string[] members, long chat_id, string group_creator, int command)
        {
            string group_name = members[1];
            string answer = "";
            reader = dbi.Select($"SELECT creator_username from groups WHERE chat_id = {chat_id} AND group_name = '{group_name}' AND creator_username = '@{group_creator}'");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                   answer += $"{reader.GetValue(0)}";
                  
                }
            }

            if (answer == $"@{group_creator}") {

                for (int i = 2; i < members.Length; i++)
                {
                    switch(command){
                        case 1:
                        dbi.Insert($"INSERT INTO members(chat_id, group_name, member) VALUES ({chat_id}, '{group_name}', '{members[i]}')");
                            break;
                        case 2:
                        dbi.Update($"DELETE FROM members WHERE member = '{members[i]}'");
                            break;
                    };
                }
            }
        
        }
        public void GroupDelete(string group_name, long chat_id, string group_creator)
        {
            group_name = group_name.Replace(" ", "");
            dbi.Update($"DELETE FROM groups WHERE group_name = '{group_name}' AND chat_id = {chat_id} AND creator_username = '@{group_creator}' ");

            reader = dbi.Select($"SELECT * FROM groups WHERE group_name = '{group_name}' AND chat_id = {chat_id} AND creator_username = '@{group_creator}' ");
            if (!reader.HasRows)
            {
                dbi.Update($"DELETE FROM members WHERE group_name = '{group_name}' AND chat_id = {chat_id} ");
            }
        }

        public string GroupTag(string group_name, long chat_id)
        {
            string answer = "";
            reader = dbi.Select($"SELECT member from members WHERE group_name = '{group_name}' AND chat_id = {chat_id}");
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    answer += $"{reader.GetValue(0)} ";

                }
            }
            return answer;
        }

        public string MyGroupList(long chat_id, string username)
        {
            string groups = "";
           reader = dbi.Select($"SELECT group_name from groups WHERE chat_id ={chat_id} AND creator_username = '@{username}'");
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    groups += $"{reader.GetValue(0)} ";
                }
            }
                    return groups;
        }

        public void GroupLeave(string group_name, long chat_id, string username)
        {
            dbi.Update($"Delete from members WHERE group_name = '{group_name}' AND chat_id = {chat_id} AND member = '@{username}'");
        }

        public string GroupsMember(long chat_id, string username)
        {
            string groups = "";
            reader = dbi.Select($"SELECT group_name from members WHERE chat_id ={chat_id} AND member = '@{username}'");
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    groups += $"{reader.GetValue(0)} ";
                }
            }
            return groups;
        }

    }
}
