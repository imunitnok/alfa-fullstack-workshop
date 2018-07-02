using System;

namespace Server.Exceptions
{
    /// <summary>
    /// Own exception class extends <see cref="System.ArgumentException"/> to impelement user data model validation errors
    /// </summary>
    public class UserDataException : ArgumentException
    {
    }
}