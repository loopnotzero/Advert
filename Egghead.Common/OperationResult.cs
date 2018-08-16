using System.Collections.Generic;
using System.Linq;

namespace Egghead.Common
{
  public class OperationResult
  {
    private static readonly OperationResult _success = new OperationResult()
    {
      Succeeded = true
    };

    private readonly List<OperationError> _errors = new List<OperationError>();
    
    /// <summary>
    /// Flag indicating whether if the operation succeeded or not.
    /// </summary>
    /// <value>True if the operation succeeded, otherwise false.</value>
    public bool Succeeded { get; protected set; }

    /// <summary>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" />s containing an errors
    /// that occurred during the identity operation.
    /// </summary>
    /// <value>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" />s.</value>
    public IEnumerable<OperationError> Errors => _errors;

    /// <summary>
    /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> indicating a successful identity operation.
    /// </summary>
    /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> indicating a successful operation.</returns>
    public static OperationResult Success => _success;

    /// <summary>
    /// Creates an <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> indicating a failed identity operation, with a list of <paramref name="errors" /> if applicable.
    /// </summary>
    /// <param name="errors">An optional array of <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" />s which caused the operation to fail.</param>
    /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> indicating a failed identity operation, with a list of <paramref name="errors" /> if applicable.</returns>
    public static OperationResult Failed(params OperationError[] errors)
    {
      OperationResult identityResult = new OperationResult()
      {
        Succeeded = false
      };
      if (errors != null)
        identityResult._errors.AddRange(errors);
      return identityResult;
    }

    /// <summary>
    /// Converts the value of the current <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> object to its equivalent string representation.
    /// </summary>
    /// <returns>A string representation of the current <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> object.</returns>
    /// <remarks>
    /// If the operation was successful the ToString() will return "Succeeded" otherwise it returned
    /// "Failed : " followed by a comma delimited list of error codes from its <see cref="P:Microsoft.AspNetCore.Identity.IdentityResult.Errors" /> collection, if any.
    /// </remarks>
    public override string ToString()
    {
      return !Succeeded ? string.Format("{0} : {1}", (object) "Failed", string.Join(",", Errors.Select(x => x.Code).ToList())) : "Succeeded";
    }
  }
}