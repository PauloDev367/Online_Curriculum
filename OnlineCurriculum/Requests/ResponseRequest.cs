namespace OnlineCurriculum.Requests;

public class ResponseRequest<T>
{
    public T? Success { get; set; }
    public List<string>? Errors { get; set; }
    public ResponseRequest()
    {
        Errors = new List<string>();
    }
    public void AddError(string error) => Errors.Add(error);
    public void SetErros(IEnumerable<string> erros) => Errors.AddRange(erros);
}