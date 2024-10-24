using DfE.DomainDrivenDesignTemplate.Infrastructure.OData;
using Microsoft.OData.Edm.Csdl;
using System.Xml;

var model = EdmModelBuilder.GetEdmModel();

var currentDirectory = AppContext.BaseDirectory;

var projectRoot = Directory.GetParent(currentDirectory)?.Parent?.Parent?.Parent?.Parent?.Parent?.FullName;

var outputPath = Path.Combine(projectRoot!, "DfE.DomainDrivenDesignTemplate.Api.Client/OData/metadata.xml");

using (FileStream fs = new FileStream(outputPath, FileMode.Create))
using (XmlWriter writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true }))
{
    var csdlWriterSettings = new CsdlXmlWriterSettings { };
    var csdlTarget = CsdlTarget.OData;

    if (CsdlWriter.TryWriteCsdl(model, writer, csdlTarget, csdlWriterSettings, out var errors))
    {
        Console.WriteLine("OData Metadata file generated successfully.");
    }
    else
    {
        foreach (var error in errors)
        {
            Console.WriteLine($"OData Metadata Error: {error.ErrorMessage}");
        }
    }
}

Console.WriteLine($"OData Metadata file generated at {outputPath}");
