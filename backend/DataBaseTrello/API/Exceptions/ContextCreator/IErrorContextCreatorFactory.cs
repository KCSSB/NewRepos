namespace API.Exceptions.ContextCreator
{
    public interface IErrorContextCreatorFactory
    {
        public ErrorContextCreator Create(string ServiceName);
    }
}
