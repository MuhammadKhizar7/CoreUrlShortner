using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreUrlShortner.Models;
[Index(nameof(Url))]
public class Url
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string code { get; set; }
    public string ShortUrl { get; set; }
    public string LongUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
