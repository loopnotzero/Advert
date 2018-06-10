namespace Egghead.Common
{
    public enum ErrorStatusCode
    {
        Ok = 200,
        Unauthorized = 401,
        NotFound = 404,
        Conflict = 409,
        UnprocessableEntity = 422,
        InternalServerError = 500,
    }
}