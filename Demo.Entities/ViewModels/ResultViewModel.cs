namespace Demo.Entities.ViewModels;

public class ResultViewModel<T>
{
    public int Status { get; private set; }

    public string Message { get; private set; }

    public T Data { get; private set; }

    public ResultViewModel(int status, string message, T data)
    {
        this.Status = status;
        this.Message = message;
        this.Data = data;
    }
}
