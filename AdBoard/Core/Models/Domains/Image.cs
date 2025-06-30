using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdBoard.Core.Models.Domains
{
    public class Image
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; }

        [ForeignKey("Ad")]
        public int AdId { get; set; }

        public Ad Ad { get; set; }
    }
}