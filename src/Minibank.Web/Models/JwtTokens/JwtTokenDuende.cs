using System;
using System.Text.Json;
using Minibank.Core.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Minibank.Core.Domains.Messages;
using System.IdentityModel.Tokens.Jwt;

namespace Minibank.Web.Models.JwtTokens
{
    public class JwtTokenDuende
    {
        public HeaderDuende Header { get; set; }
        public PayloadDuende Payload { get; set; }

        public void EnsureTokenIsNotExpiredOrThrow()
        {
            if (Payload.TokenExpirationDate < DateTime.Now)
                throw new CustomAuthenticationException(
                    ErrorMessages.GetAuthenticationErrorMessage(
                        ErrorMessages.AuthenticationErrorLegend, 
                        ErrorMessages.JwtTokenExpiredErrorReason));
        }

        public static bool TryParse(string jwtTokenString, out JwtTokenDuende jwtToken)
        {
            if (jwtTokenString == null)
            {
                jwtToken = null;
                return false;
            }

            jwtToken = new JwtTokenDuende();
            var token = new JwtSecurityToken(jwtTokenString[7..]);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeToUnixConverter());

            jwtToken.Header = JsonSerializer.Deserialize<HeaderDuende>(token.Header.SerializeToJson(), options);
            jwtToken.Payload = JsonSerializer.Deserialize<PayloadDuende>(token.Payload.SerializeToJson(), options);

            return jwtToken != null;
        }

        public class HeaderDuende
        {
            [JsonPropertyName("alg")]
            public string Algoritm { get; set; }

            [JsonPropertyName("kid")]
            public string KeyId { get; set; }

            [JsonPropertyName("typ")]
            public string TokenType { get; set; }
        }

        public class PayloadDuende
        {
            [JsonPropertyName("iss")]
            public string AuthorizationServer { get; set; }

            [JsonPropertyName("nbf")]
            public DateTime TokenPrelockDate { get; set; }

            [JsonPropertyName("iat")]
            public DateTime TokenIssueDate { get; set; }

            [JsonPropertyName("exp")]
            public DateTime TokenExpirationDate { get; set; }

            [JsonPropertyName("aud")]
            public List<string> TokenRecipents { get; set; }

            [JsonPropertyName("scope")]
            public List<string> Scope { get; set; }

            [JsonPropertyName("client_id")]
            public string ClientId { get; set; }

            [JsonPropertyName("jti")]
            public string TokenEmitentsId { get; set; }
        }

        public class DateTimeToUnixConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64()).DateTime.ToLocalTime();
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
