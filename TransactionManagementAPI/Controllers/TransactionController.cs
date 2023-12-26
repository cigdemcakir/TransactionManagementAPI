using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TransactionManagementAPI.Entity;

namespace TransactionManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class TransactionController : ControllerBase
{
    public TransactionController()
    {
            
    }
    
    private static List<Transaction> _transactions = new()
    {
        new Transaction()
        {
            Id = 1,
            FromAccountId = 100,
            ToAccountId = 200,
            Amount = 500.00M,
            TransactionDate = new DateTime(2023, 12, 1),
            Description = "Rent Payment"
        },
        new Transaction
        {
            Id = 2,
            FromAccountId = 200,
            ToAccountId = 300,
            Amount = 1500.00M,
            TransactionDate = new DateTime(2023, 12, 2),
            Description = "Salary Deposit"
        },
        new Transaction
        {
            Id = 3,
            FromAccountId = 300,
            ToAccountId = 400,
            Amount = 250.00M,
            TransactionDate = new DateTime(2023, 12, 3),
            Description = "Bill Payment"
        },
        new Transaction
        {
            Id = 4,
            FromAccountId = 400,
            ToAccountId = 500,
            Amount = 1000.00M,
            TransactionDate = new DateTime(2023, 12, 4),
            Description = "Charity Donation"
        }
    };
    
    // GET: Lists all transactions
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_transactions);
    }
    
    // GET: Retrieves a transaction by ID
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == id);
        
        if (transaction == null) return NotFound(); // 404 Not Found
        
        return Ok(transaction); // 200 OK
    }
    
    // POST: Creates a new transaction
    [HttpPost]
    public IActionResult Create([FromBody] Transaction transaction)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState); // 400 Bad Request
        
        var newId = _transactions.Any() ? _transactions.Max(t => t.Id) + 1 : 1;
        
        transaction.Id = newId;

        _transactions.Add(transaction);
        
        Response.Headers.Add("X-Transaction-Status", "Transaction with ID " + transaction.Id + " has been successfully created.");

        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction); // 201 Created
    }

    // PUT: Updates an existing transaction
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Transaction transaction)
    {
        var existingTransaction = _transactions.FirstOrDefault(t => t.Id == id);
        
        if (existingTransaction == null) return NotFound();

        if (!ModelState.IsValid) return BadRequest(ModelState);

        existingTransaction.FromAccountId = transaction.FromAccountId;
        existingTransaction.ToAccountId = transaction.ToAccountId;
        existingTransaction.Amount = transaction.Amount;
        existingTransaction.TransactionDate = transaction.TransactionDate;
        existingTransaction.Description = transaction.Description;
        
        Response.Headers.Add("X-Transaction-Status", "Transaction with ID " + id + " has been successfully updated.");

        return Ok(existingTransaction);
    }

    // DELETE: Deletes a transaction
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == id);
        
        if (transaction == null) return NotFound("Transaction not found.");

        _transactions.Remove(transaction);
        
        Response.Headers.Add("X-Transaction-Status", "Transaction with ID " + id + " has been successfully deleted.");
        
        return NoContent(); // 204 No Content

    }
    
    // PATCH: Partially updates a specific field of a transaction
    [HttpPatch("{id}")]
    public IActionResult Patch(int id, [FromBody] JsonPatchDocument<Transaction> patchDoc)
    {
        if (patchDoc == null)
            return BadRequest();

        var transaction = _transactions.FirstOrDefault(t => t.Id == id);
        
        if (transaction == null) return NotFound("Transaction not found.");

        patchDoc.ApplyTo(transaction, ModelState);

       if (!ModelState.IsValid) return BadRequest(ModelState);
       
       var response = new
       {
           Message = $"Transaction with ID {id} has been updated.",
           UpdatedTransaction = transaction
       };

        return Ok(response);
    }

    // Extra: Filters and lists transactions based on description
    [HttpGet("filter")]
    public IActionResult FilterTransactions([FromQuery] string filter)
    {
        var filteredTransactions = _transactions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            filteredTransactions = filteredTransactions.Where(t => t.Description.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(filteredTransactions.ToList());
    }
    
    [HttpGet("sort")]
    public IActionResult SortTransactions([FromQuery] string sortBy)
    {
        var sortedTransactions = _transactions.AsQueryable();

        switch (sortBy?.ToLower())
        {
            case "date":
                sortedTransactions = sortedTransactions.OrderBy(t => t.TransactionDate);
                break;
            case "amount":
                sortedTransactions = sortedTransactions.OrderBy(t => t.Amount);
                break;
            default:
                return BadRequest("Invalid sort parameter");
        }

        return Ok(sortedTransactions.ToList());
    }

}
    
