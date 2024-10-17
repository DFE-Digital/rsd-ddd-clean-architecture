using System.ComponentModel.DataAnnotations.Schema;

namespace DfE.DomainDrivenDesignTemplate.Domain.ValueObjects
{
    public record NameDetails(string? NameListAs, string? NameDisplayAs, string? NameFullTitle);
}
