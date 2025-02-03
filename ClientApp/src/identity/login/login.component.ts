import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import { loginUser } from '../models/loginUser';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  constructor(
    private authenticationService: AuthenticationService,
    private router: Router
  ) {}

  loginForm = new FormGroup({
    email: new FormControl('', Validators.required),
    password: new FormControl('', [Validators.required]),
  });

  submitForm() {
    if (this.loginForm.valid) {
      let formControls = this.loginForm.controls;
      console.log('valid');
      let user = new loginUser(
        this.loginForm.controls.email.value!,
        this.loginForm.controls.password.value!
      );

      this.authenticationService.login(user).subscribe({
        next: (response) => {
          this.router.navigate(['/temperature']);
        },
        error: (err) => {
          console.error('Login failed:', err);
        },
      });
    }
  }
}
