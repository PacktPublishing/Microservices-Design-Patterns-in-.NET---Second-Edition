using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using DocumentsService.Protos;

public class DocumentServiceImpl : DocumentService.DocumentServiceBase
{
    // Simulating a data store
    private static readonly List<Document> Documents = new List<Document>
    {
        new Document { Id = "1", PatientId = "123", Name = "Document1" },
        new Document { Id = "2", PatientId = "456", Name = "Document2" }
    };

    public override Task<DocumentList> GetAll(PatientId request, ServerCallContext context)
    {
        var documentList = new DocumentList();
        documentList.Documents.AddRange(Documents.Where(q => q.Id == request.Id));
        return Task.FromResult(documentList);
    }

    public override Task<Document> Get(DocumentId request, ServerCallContext context)
    {
        var document = Documents.Find(doc => doc.Id == request.Id);
        if (document == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Document not found"));
        }
        return Task.FromResult(document);
    }
}
