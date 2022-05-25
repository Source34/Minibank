namespace Minibank.Core.Domains.BankAccounts.Validators
{
    public static class BankAccountValidatorMessages
    {
        public const string InvalidCurrencyCode = "Недопустимый код валюты!";
        public const string NotEmptyBalance = "Что бы закрыть аккаунт, необходимо перевести все средства со счета!";
        public const string BankAccountAlreadyClosed = "Аккаунт уже закрыт!";
    }
}
