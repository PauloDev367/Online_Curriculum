namespace OnlineCurriculum.Requests;

public class CreatedBucketResponse
{
    public string FileName { get; set; }
    public string OriginalName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
}