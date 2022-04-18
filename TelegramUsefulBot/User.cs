using TelegramUsefulBot.UserStates;

namespace TelegramUsefulBot
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public StateMachine State { get; set; }

        public User()
        {
            State = new StateMachine(this, new DefaultState());
        }
    }
}
