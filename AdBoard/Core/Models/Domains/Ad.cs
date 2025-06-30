using AdBoard.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdBoard.Core.Models.Domains
{
    public class Ad
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public static readonly List<string> Categories =
        [
            "Sprzedam",
            "Kupię",
            "Wynajmę komuś",
            "Wynajmę od kogoś",
            "Zatrudnię na stałe",
            "Zatrudnię dorywczo",
            "Szukam stałej pracy",
            "Szukam dorywczej pracy",
            "Oddam za darmo",
            "Przyjmę za darmo",
            "Usługi",
            "Wydarzenia",
            "Poznam panią",
            "Poznam pana",
            "Zgubiono rzecz",
            "Zwierzę się zgubiło",
            "Człowiek się zgubił",
            "Inne"
        ];

        public static readonly List<string> Units =
        [
            "zł",
            "zł / szt.",
            "zł / kg",
            "zł / t",
            "zł / l",
            "zł / m",
            "zł / m^2",
            "zł / ha",
            "zł / m^3",
            "zł / km",
            "zł / h",
            "zł / dzień",
            "zł / tydzień",
            "zł / miesiąc",
            "€",
            "€ / h",
            "€ / dzień",
            "€ / tydzień",
            "€ / miesiąc"
        ];

        [MaxLength(22)]
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Kategoria")]
        public string Category { get; set; }
        
        [MaxLength(80, ErrorMessage = "Tytuł może mieć maksymalnie 80 znaków.")]
        [Required(ErrorMessage = "Pole \"{0}\" jest wymagane.")]
        [Display(Name = "Tytuł")]
        public string Title { get; set; }

        [Display(Name = "Data")]
        public DateOnly CreatedDate { get; set; }

        [Display(Name = "Opis")]
        public string? Description { get; set; }

        [Display(Name = "Kwota")]
        [Column(TypeName = "decimal(18,2)")]
        [RangeIfNotNull(0.01, 9999999999999999.99, ErrorMessage = "{0} musi być liczbą z zakresu od {1} do {2}.")]
        public decimal? Value { get; set; }

        [MaxLength(12)] 
        public string Unit { get; set; }

        public ApplicationUser User { get; set; }

        public List<Image> Images { get; set; } = [];
    }
}