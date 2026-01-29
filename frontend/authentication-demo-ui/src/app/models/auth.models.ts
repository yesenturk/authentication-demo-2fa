export interface LoginRequest {
  username: string;
  password: string;
  deviceToken?: string;
}

export interface VerifyTotpRequest {
  username: string;
  totpCode: string;
  deviceToken?: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  token?: string;
  requiresTwoFactor: boolean;
  qrCodeImage?: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
}

export interface ForgetDeviceRequest {
  deviceToken: string;
}