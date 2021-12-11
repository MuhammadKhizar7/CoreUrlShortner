using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreUrlShortner.Models;

public class Link
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(7)]
    [Column(TypeName = "VARCHAR")]
    public string code { get; set; } = default!;
    [Required]
    [StringLength(500, ErrorMessage ="URL is too long")]
    public string LongUrl { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
