namespace Egghead.Common
{
    public enum ErrorStatusCode
    {
        Ok = 200,
        Created = 201,
        Found = 302,
        Unauthorized = 401,
        NotFound = 404,
        Conflict = 409,
        UnprocessableEntity = 422,
        InternalServerError = 500,
    }
}