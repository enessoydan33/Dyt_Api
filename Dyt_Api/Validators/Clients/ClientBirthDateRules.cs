namespace Dyt_Api.Validators.Clients
{
    public static class ClientBirthDateRules
    {
        public const string FutureDateMessage = "BirthDate cannot be in the future.";
        public const string MaximumAgeMessage = "Client age cannot be greater than 120.";

        private const int MaximumAge = 120;

        public static bool IsNotInFuture(DateOnly birthDate)
        {
            return birthDate <= BusinessDate.Today;
        }

        public static bool IsWithinMaximumAge(DateOnly birthDate)
        {
            return !IsNotInFuture(birthDate) || BusinessDate.Today < birthDate.AddYears(MaximumAge + 1);
        }
    }
}
