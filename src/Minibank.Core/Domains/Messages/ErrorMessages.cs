using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Messages
{
    public static class ErrorMessages
    {
        public const string CreatingErrorLegend = "Ошибка создания!";
        public const string UpdatingErrorLegend = "Ошибка обновления!";
        public const string DeletingErrorLegend = "Ошибка удаления!";
        public const string GettingByIdErrorLegend = "Ошибка получения!";
        public const string AuthenticationErrorLegend = "Ошибка аутенификации!";
        public const string ClosingBankAccountErrorLegend = "Ошибка закрытия банковского аккаунта!";

        public const string UserLoginOrEmailIsNotUniqe = "Пользователь с таким логином и/или email уже существует!";
        public const string JwtTokenExpiredErrorReason = "Ошибка аутенификации! Jwt-токен просрочен!";
        public const string JwtTokenParsingErrorReason = "Не удалось распарсить Jwt-токен!";
        public static string GetObjectNotFoundErrorMessage(string actionErrorLegend, Type objectType, int objectId)
        {
            return $"{actionErrorLegend} Объект {objectType} c Id = {objectId} не найден!".TrimStart(' ');
        }

        public static string GetUserDeletingWithOpenBankAccountErrorMessage(int userId)
        {
            return $"{DeletingErrorLegend} {typeof(User)} c Id = {userId} имеет открытые банковские аккаунты!";
        }

        public static string GetAuthenticationErrorMessage(string actionErrorLegend, string reason)
        {
            return $"{actionErrorLegend} {reason}";
        }
    }
}
