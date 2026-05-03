using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.Helpers.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {

        }
    }

    public class LogicalErrorException : CustomException
    {
        public LogicalErrorException(string message) : base(message)
        {
            
        }
    }

    public class RequiredFieldException : CustomException
    {
        public RequiredFieldException(string message) : base(message)
        {

        }
    }

    public class DuplicateEntityException : CustomException
    {
        public DuplicateEntityException(string message) : base(message)
        {

        }
    }

    public class ActiveUserDeletionException : CustomException
    {
        public ActiveUserDeletionException(string message) : base(message)
        {

        }
    }

    public class NotFoundException : CustomException
    {
        public NotFoundException(string message) : base(message)
        {
            
        }
    }
}
