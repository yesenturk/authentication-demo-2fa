import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import {
  LoginRequest,
  VerifyTotpRequest,
  AuthResponse,
  RegisterRequest,
  ForgetDeviceRequest
} from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private tokenKey = 'auth_token';
  private deviceTokenKey = 'device_token';
  private usernameKey = 'temp_username';
  
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Device Token oluştur ve sakla
  getOrCreateDeviceToken(): string {
    let deviceToken = localStorage.getItem(this.deviceTokenKey);
    if (!deviceToken) {
      deviceToken = this.generateGuid();
      localStorage.setItem(this.deviceTokenKey, deviceToken);
    }
    return deviceToken;
  }

  // GUID oluştur
  private generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  // JWT'den userId çıkar
  getUserIdFromToken(): number | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return parseInt(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']);
    } catch (error) {
      return null;
    }
  }

  // JWT'den username çıkar
  getUsernameFromToken(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    } catch (error) {
      return null;
    }
  }

  // Login
  login(request: LoginRequest): Observable<AuthResponse> {
    request.deviceToken = this.getOrCreateDeviceToken();
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, request)
      .pipe(
        tap(response => {
          if (response.success && response.token) {
            this.setToken(response.token);
            this.isAuthenticatedSubject.next(true);
          } else if (response.success && response.requiresTwoFactor) {
            localStorage.setItem(this.usernameKey, request.username);
          }
        })
      );
  }

  // 2FA Doğrulama
  verifyTotp(request: VerifyTotpRequest): Observable<AuthResponse> {
    request.deviceToken = this.getOrCreateDeviceToken();
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/verify-totp`, request)
      .pipe(
        tap(response => {
          if (response.success && response.token) {
            this.setToken(response.token);
            this.isAuthenticatedSubject.next(true);
            localStorage.removeItem(this.usernameKey);
          }
        })
      );
  }

  // Register
  register(request: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/register`, request);
  }

  // Cihazı Unut
  forgetDevice(userId: number): Observable<any> {
    const deviceToken = localStorage.getItem(this.deviceTokenKey);
    if (!deviceToken) {
      throw new Error('Device token bulunamadı!');
    }

    const request: ForgetDeviceRequest = { deviceToken };
    
    // Backend'e userId de gönderelim (body içinde veya endpoint'te)
    // Şimdilik basit tutup body'de gönderelim
    return this.http.post(`${this.apiUrl}/auth/forget-device`, { 
      deviceToken, 
      userId 
    });
  }

  // Logout
  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.usernameKey);
    this.isAuthenticatedSubject.next(false);
  }

  // Token işlemleri
  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private hasToken(): boolean {
    return !!this.getToken();
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  getTempUsername(): string | null {
    return localStorage.getItem(this.usernameKey);
  }
}