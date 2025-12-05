using System.ComponentModel.DataAnnotations;

namespace ExpandeBO_Backend.Models.DTOs;

public class RegistroEmpresaRequest
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres")]
    public string Apellido { get; set; } = string.Empty;
    
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe tener exactamente 8 dígitos")]
    public string? Telefono { get; set; }
    
    [Required(ErrorMessage = "La razón social es requerida")]
    [MaxLength(50, ErrorMessage = "La razón social no puede exceder 50 caracteres")]
    public string RazonSocial { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El NIT es requerido")]
    [RegularExpression(@"^\d{7,12}$", ErrorMessage = "El NIT debe tener entre 7 y 12 dígitos numéricos")]
    public string NIT { get; set; } = string.Empty;
    
    [MaxLength(50, ErrorMessage = "La dirección no puede exceder 50 caracteres")]
    public string? Direccion { get; set; }
}


