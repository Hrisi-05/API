namespace UserAPI.DTOs
{
    public enum ServiceError { None, NotFound,Conflict, Unauthorized }
    public class ServiceResult<T>
    {
        public T? Data { get; private init; }
        public ServiceError Error { get; private init; }
        public string? Message { get; private init; }
        public bool Success => Error == ServiceError.None;

        public static ServiceResult<T> Ok(T data) => new() { Data = data };

        public static ServiceResult<T> Fail(ServiceError error, string message) =>
            new() { Error = error, Message = message };
    }
}
