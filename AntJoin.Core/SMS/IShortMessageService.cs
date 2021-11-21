namespace AntJoin.Core.SMS
{
    public interface IShortMessageService
    {
        bool SendMessage(string phone, string content, string sign, out string msg);
    }
}
