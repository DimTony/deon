var userId = int.Parse(jwtToken.Claims.First(x => x.Subject == jwtToken.Subject).Subject); error CS0019: Operator '==' cannot be applied to operands of type 'ClaimsIdentity' and 'string'
