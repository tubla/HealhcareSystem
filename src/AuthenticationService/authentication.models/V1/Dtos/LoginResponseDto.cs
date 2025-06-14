﻿namespace authentication.models.V1.Dtos;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}
