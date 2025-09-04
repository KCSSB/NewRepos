namespace API.Exceptions.ContextCreator
{
    public class ErrorContextCreatorFactory: IErrorContextCreatorFactory
    {
        public ErrorContextCreator Create(string serviceName)
        {
            return new ErrorContextCreator(serviceName);
        }
    }
}
