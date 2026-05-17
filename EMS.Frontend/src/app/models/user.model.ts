//src/app/models/user.model.ts
export interface User {
  emailId: string;
  userName: string;
  role: string;
  isActive: boolean;
  createdAt: Date;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  userName: string;
  password: string;
  confirmPassword: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  data: {
    accessToken: string;
    tokenType: string;
    expiresIn: number;
  };
  statusCode: number;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  statusCode: number;
}
