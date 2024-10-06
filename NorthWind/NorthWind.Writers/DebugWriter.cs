using NorthWind.Entities.Interfaces;
using NorthWind.Entities.ValueObjects;
using System.Diagnostics;

namespace NorthWind.Writers
{
    public class DebugWriter : IUserActionWriter
    {
        public void Write(UserAction action)
        {
           Debug.WriteLine("ConsoleWriter: {0}, {1}, {2}",
                action.CreatedDateTime, action.User, action.Description);
        }


    }
}
