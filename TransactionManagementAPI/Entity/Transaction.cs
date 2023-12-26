using System.ComponentModel.DataAnnotations;

namespace TransactionManagementAPI.Entity;

public class Transaction
{
    public int Id { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "FromAccountId must be a positive value.")]
    public int FromAccountId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ToAccountId must be a positive value.")]
    public int ToAccountId { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value greater than zero.")]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime TransactionDate { get; set; }
    
    public string Description { get; set; }
}