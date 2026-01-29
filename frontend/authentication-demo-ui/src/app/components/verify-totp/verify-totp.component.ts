import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-verify-totp',
  templateUrl: './verify-totp.component.html',
  styleUrls: ['./verify-totp.component.css']
})
export class VerifyTotpComponent implements OnInit {
  verifyForm!: FormGroup;
  loading = false;
  errorMessage = '';
  qrCodeImage = '';
  username = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    // Router state'den QR code'u al
    const navigation = this.router.getCurrentNavigation();
    const state = navigation?.extras.state as { qrCodeImage: string };
    
    if (state && state.qrCodeImage) {
      this.qrCodeImage = state.qrCodeImage;
    }
  }

  ngOnInit(): void {
    // Username'i al
    this.username = this.authService.getTempUsername() || '';
    
    if (!this.username || !this.qrCodeImage) {
      // Gerekli bilgiler yoksa login'e dön
      this.router.navigate(['/login']);
      return;
    }

    this.verifyForm = this.formBuilder.group({
      totpCode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });
  }

  get f() {
    return this.verifyForm.controls;
  }

  onSubmit(): void {
    this.errorMessage = '';

    if (this.verifyForm.invalid) {
      return;
    }

    this.loading = true;

    const request = {
      username: this.username,
      totpCode: this.verifyForm.value.totpCode
    };

    this.authService.verifyTotp(request).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.router.navigate(['/home']);
        } else {
          this.errorMessage = response.message;
        }
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error.error?.message || 'Doğrulama başarısız!';
      }
    });
  }

  onCodeInput(event: any): void {
    // Sadece rakam girişine izin ver
    const value = event.target.value.replace(/\D/g, '');
    this.verifyForm.patchValue({ totpCode: value });
  }

  cancel(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}