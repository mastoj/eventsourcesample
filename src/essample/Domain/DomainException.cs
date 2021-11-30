using System;

namespace essample.Domain
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) {}    
    }
}