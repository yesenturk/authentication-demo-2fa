import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  username = 'Kullanıcı';
  userId = 0;
  currentDate = new Date();
  showForgetModal = false;
  forgetLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.username = this.authService.getUsernameFromToken() || 'Kullanıcı';
    this.userId = this.authService.getUserIdFromToken() || 0;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  openForgetModal(): void {
    this.showForgetModal = true;
  }

  closeForgetModal(): void {
    this.showForgetModal = false;
  }

  confirmForgetDevice(): void {
    if (this.userId === 0) {
      alert('Kullanıcı bilgisi alınamadı!');
      return;
    }

    this.forgetLoading = true;

    this.authService.forgetDevice(this.userId).subscribe({
      next: () => {
        this.forgetLoading = false;
        this.showForgetModal = false;
        
        alert('Cihaz başarıyla unutuldu! Bir sonraki girişte 2FA kodu istenecek.');
        this.logout();
      },
      error: (error) => {
        this.forgetLoading = false;
        alert('Bir hata oluştu: ' + (error.error?.message || 'Bilinmeyen hata'));
      }
    });
  }
}