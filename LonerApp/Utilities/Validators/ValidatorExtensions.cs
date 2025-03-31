using FluentValidation;
namespace LonerApp.Validators
{
    public static class ValidatorExtensions
    {
        private const string PHONE_REGEX_PATTERN = @"^0[0-9]+$";
        private const string EMAIL_REGEX_PATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private const int PHONE_LENGTH = 10;
        private const int EMAIl_LENGTH = 50;
        private const int VERIFY_CODE_LENGTH = 6;

        public static IRuleBuilderOptions<T, string> FiledNotEmpty<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(I18nHelper.Get("Common_Error_NotEmpty"));
        }

        public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Matches(PHONE_REGEX_PATTERN)
                .WithMessage(I18nHelper.Get("Common_Error_Phone"))
                .Length(PHONE_LENGTH)
                .WithMessage(I18nHelper.Get("Common_Error_PhoneLength"));
        }

        public static IRuleBuilderOptions<T, string> VerifyCode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Length(VERIFY_CODE_LENGTH)
                .WithMessage(I18nHelper.Get("Common_Error_VerifyLength"));
        }

        public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Matches(EMAIL_REGEX_PATTERN)
                .WithMessage(I18nHelper.Get("Common_Error_Email"))
                .MaximumLength(EMAIl_LENGTH)
                .WithMessage(I18nHelper.Get("Common_Error_EmailLength"));
        }

        public static IRuleBuilderOptions<T, string> Date<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .FiledNotEmpty()
                .Must(value => int.TryParse(value, out _)).WithMessage(I18nHelper.Get("Common_Error_Date_Is_Number"))
                .Must(value => int.TryParse(value, out int day) && day >= 1 && day <= 31)
                .WithMessage(I18nHelper.Get("Common_Error_Date"));
        }

        public static IRuleBuilderOptions<T, string> Month<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .FiledNotEmpty()
                .Must(value => int.TryParse(value, out _)).WithMessage(I18nHelper.Get("Common_Error_Month_Is_Number"))
                .Must(value => int.TryParse(value, out int month) && month >= 1 && month <= 12)
                .WithMessage(I18nHelper.Get("Common_Error_Month"));
        }

        public static IRuleBuilderOptions<T, string> Year<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .FiledNotEmpty()
                .Must(value => int.TryParse(value, out _)).WithMessage(I18nHelper.Get("Common_Error_Year_Is_Number"))
                .Must(value => int.TryParse(value, out int year) && year >= 1950 && year <= 2003)
                .WithMessage(I18nHelper.Get("Common_Error_Year"));
        }
    }
}