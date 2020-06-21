using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZipStore.Entities
{
    [DisplayName("Запчасть")]
    public class ZipItem
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Необходим производитель", AllowEmptyStrings = false)]
        [Display(Name = "Производитель")]
        [StringLength(64)]
        public string Vendor { get; set; }
        
        [Required(ErrorMessage = "Необходим номер запчасти", AllowEmptyStrings = false)]
        [Display(Name = "Номер запчасти")]
        [StringLength(64)]
        public string Number { get; set; }
        
        [Required(ErrorMessage = "Необходим производитель для поиска", AllowEmptyStrings = false)]
        [Display(Name = "Производитель для Поиска")]
        [StringLength(64)]
        public string SearchVendor { get; set; }
        
        [Required(ErrorMessage = "Необходим номер для поиска", AllowEmptyStrings = false)]
        [Display(Name = "Номер для Поиска")]
        [StringLength(64)]
        public string SearchNumber { get; set; }
        
        [Required(ErrorMessage = "Необходимо описание запчасти", AllowEmptyStrings = false)]
        [Display(Name = "Нименование")]
        [StringLength(512)]
        public string Description { get; set; }
        
        [Required]
        [Display(Name = "Цена")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        public double Price { get; set; }
        
        [Required]
        [Display(Name = "Количество")]
        public int Count { get; set; }

    }
}