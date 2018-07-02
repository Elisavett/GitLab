namespace GitLabDotNet
{
    public class GitLabServerErrorException : System.Exception
    {
        public int ErrorCode
        {
            get
            {
                return _ErrorCode;
            }
        }

        private int _ErrorCode;

        new public string Message
        {
            get { return _Message; }
        }

        private string _Message;

        public GitLabServerErrorException(string _message, int _errorcode): base(_message)
        {            
            this._Message = _message;
            this._ErrorCode = _errorcode;
        }
    }

    public class GitLabStaticAccessException : System.Exception
    {
        new public string Message
        {
            get { return _Message; }
        }

        private string _Message;

        public GitLabStaticAccessException(string _message): base(_message)
        {
            this._Message = _message;
        }
    }
}
