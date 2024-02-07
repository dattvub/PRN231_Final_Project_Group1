using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace PDMS.Models;

public class ValidationError {
    public int StatusCode { get; set; }
    public string ReasonPhrase { get; set; }
    public List<string> Errors { get; set; }
    public DateTime Timestamp { get; set; }

    public static IActionResult GenerateErrorResponse(ActionContext context) {
        var errorList = new List<string>();

        var errors = context.ModelState.AsEnumerable();
        foreach (var error in errors) {
            if (error.Value == null) {
                continue;
            }

            errorList.AddRange(error.Value.Errors.Select(inner => inner.ErrorMessage));
        }

        return BadRequest400(errorList.ToArray());
    }

    public static ObjectResult BadRequest400(params string[] errors) {
        var error = new ValidationError() {
            StatusCode = StatusCodes.Status400BadRequest,
            ReasonPhrase = ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest),
            Errors = new List<string>(errors),
            Timestamp = DateTime.Now
        };
        return new BadRequestObjectResult(error);
    }

    public static ObjectResult InternalServerError500(params string[] errors) {
        var error = new ValidationError() {
            StatusCode = StatusCodes.Status500InternalServerError,
            ReasonPhrase = ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError),
            Errors = new List<string>(errors),
            Timestamp = DateTime.Now
        };
        return new ObjectResult(error) {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}